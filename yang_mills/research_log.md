# Research log

## 2026-09-25 — Iteration 1

Idea:
Reduce the gap half of the problem to a physical-scale Euclidean-time clustering estimate and investigate whether functional inequalities can supply it.

Mathematical setup:
Wilson lattice measure on compact link variables with Haar measure; avoid gauge fixing for the main reduction. Use reflection positivity to obtain transfer evolution. Separate the existence/UV problem from the IR gap problem.

Verified:
1. Spectral theorem proves YM-BRIDGE-1: a common exponential Euclidean-time decay rate on a dense centered local algebra excludes spectrum below that rate.
2. Correct lattice scaling target is gamma(a)>=m_* a. Requiring a lattice-unit exponent bounded below by a positive constant independent of a would be much stronger than needed.
3. Strong-coupling SU(N) result of Shen–R. Zhu–X. Zhu gives LSI/PI and exponential correlation decay only for |beta|<1/[16(d-1)]. In d=4: |beta|<1/48.
4. Their proof shows PI alone is not the spatial-mixing mechanism: PI gives decay in auxiliary semigroup time, while commutator/locality bounds control propagation; optimizing the two yields spatial decay.
5. This suggests a scale-invariant target lambda_a/v_a >= c a Lambda_YM.
6. Direct Bakry–Emery continuation fails because K_S becomes negative as beta grows.
7. Pointwise finite-beta gap, analytic continuation, no finite-beta phase transition, area law, or Lambda_YM>0 alone do not imply the required continuum spectral gap.

Problem encountered:
No weak-coupling, volume-uniform estimate with correct a(beta) scaling was obtained.

Result:
STATUS: PROMISING ROUTE — NOT A PROOF.

Next step:
Prove an abstract PI+finite-propagation => covariance theorem with explicit constants, then search for an RG mechanism that gives lambda_a/v_a at the asymptotically-free trajectory without assuming Dobrushin mixing as an input.


## 2026-09-25 — Iteration 1b

New theorem:
YM-SEMIGROUP-1. Under a Poincare variance decay and a weighted gradient-propagation bound for a local reversible diffusion, covariance decays in lattice distance with rate at least
kappa lambda/[2(lambda+v_kappa)].
This makes explicit the hidden second ingredient behind the Shen–Zhu–Zhu strong-coupling argument.

Adversarial finding:
A global auxiliary Markov spectral gap is not obviously a minimal target. Global/topological bottlenecks may make it small without forcing long-range local physical correlations.

New reduced route:
YM-RG-LANDING-1. Run exact RG from the asymptotically-free UV and prove that at a_* comparable to Lambda_YM^{-1} the effective polymer activities obey a uniform Kotecky-Preiss exponential norm bound. Standard cluster expansion would then yield an O(Lambda_YM) correlation rate. Reflection positivity can remain entirely at the original Wilson lattice level; it need not be preserved by each blocking transformation.

Current best next attack:
Make YM-RG-LANDING-1 less black-box by splitting it into:
(a) UV Balaban-controlled steps up to a matching scale;
(b) a finite crossover lemma for the remaining O(1) range of effective coupling;
(c) entry into a rigorous strong-mixing/polymer domain.
The likely genuinely new content is (b).


## 2026-09-25 — Iteration 1c

Reduction sharpened:
Introduce YM-CROSSOVER-1. After the UV RG reaches a fixed matching coupling g_match, prove that a finite, cutoff-independent number M_G of exact RG steps sends the full closed/bounded matching tube of effective interactions into a Kotecky-Preiss strong-mixing domain.

Reason this matters:
The number of UV steps diverges as a->0, but those are the asymptotically-free/small-coupling steps for which constructive RG is the natural tool. The genuinely nonperturbative crossover starts from a fixed g_match, so its required scale interval is fixed; if it can be controlled, it is a finite-step problem.

Adversarial warning:
A scalar running coupling is not enough. The inclusion must be in a norm controlling the entire generated effective action, including irrelevant operators and large-field/polymer terms.


## 2026-09-25 — Iteration 2: YM-CROSSOVER-1 only

Adversarial result 1:
The previous reduction was too optimistic if it tracked only a running coupling. Explicit toy RG maps show that a coupling can run monotonically to a nominal strong-coupling region while generated irrelevant/polymer coordinates grow or settle outside every small-activity domain.

Adversarial result 2:
Measure-only landing in a KP domain does not by itself control original microscopic correlations. The exact conditional-covariance decomposition exposes the missing term. A hidden-sector toy model gives a direct counterexample. Added YM-RG-SOURCE-1.

Concrete state space:
Defined B_{alpha,mu,p,rho}, a weighted Banach space of globally defined gauge-invariant interactions on connected 4D block polymers. Local norms are explicit C^p norms built from invariant Lie derivatives and supremized over collar/boundary link fields. See crossover_spec.md.

Concrete landing domain:
Defined exact scalar super-polymer activities by expanding the global interaction, grouping connected overlapping families and integrating against product Haar. Proved that
Q_{alpha,mu}<alpha
implies the Kotecky-Preiss condition and uniform cluster expansion.

Finite-step reduction:
Proved YM-TUBE-1. YM-CROSSOVER measure inclusion follows from a finite list of one-step interval enclosures plus a final KP inequality.
Proved YM-RG-DRIFT-1. A uniform drift delta>0 together with an invariant remainder tube implies a cutoff-independent finite number of crossover steps; finite M is therefore no longer assumed.

Matching correction:
Balaban's verified published claims provide a substantial UV RG package, but do not automatically give the specific closed/bounded K_match in the new global norm. Added YM-MATCH-EXTRACT-1 as a separate unresolved input.

Continuum bridge audit:
Strengthened the cutoff bridge to YM-CUTOFF-BRIDGE-2. Once each fixed-cutoff theory has a genuine transfer gap m_a, the semigroup inequality
C_a(t+s)<=e^{-m_a s}C_a(t)
passes to the continuum using positive-time correlator convergence; no equal-time norm convergence or uniform cluster prefactor is required.

Validated numerics:
Added scripts/verify_crossover_tube.py, an exact Fraction-based interval checker, and a toy certificate. It certifies only the finite induction conditional on genuine RG enclosure formulae.

Result:
STATUS: YM-CROSSOVER-1 REDUCED TO LEMMAS A/B/C — NOT PROVED.

A = YM-MATCH-EXTRACT-1.
B = Yang-Mills one-step invariant-tube/drift enclosures ending in Q_KP<alpha.
C = YM-RG-SOURCE-1.

Next action:
Do not open a new mass-gap route. Extract an actual one-step Balaban gauge-RG map/bounds into the variables beta,r of crossover_spec.md and determine whether the first nontrivial tube step can be proved analytically.


### Iteration 2 final refinement

Proved YM-RG-DERIV-1:
the first and second Frechet derivatives of exact block integration are conditional expectations and minus conditional covariances.

Proved YM-RG-TAYLOR-1:
a full infinite-dimensional one-step tube inclusion is implied by five scalar families of rigorous bounds (center flow, generated remainder, relevant/irrelevant mixing, irrelevant amplification, second derivative). This is the closest result of this iteration to the target
YM-CROSSOVER-1 <=> finite explicit list of inequalities.

Exact continuation point:
extract the first matching interval I_0 and radius R_0 from Balaban-type bounds, then prove numerical/analytic enclosures f_0^±,H_0,A_0,B_0,C_0. Do not move to another mass-gap route before this first tube step is either certified or shown impossible in the chosen norm.


## 2026-09-25 — Iteration 3: YM-MATCH-EXTRACT-1 + STEP_0 validity only

Compactness audit:
The earlier description of K_match as compact in an infinite-dimensional Banach space was incorrect. The matching object is now only closed/bounded. All tube results use universal inequalities/suprema and do not need attainment.

Source/topology audit:
Equation-level source extraction shows that Balaban's effective density is domain-history dependent and includes characteristic/T-operation structure. The E/R/B pieces are localized and analytic on restricted regularity domains; large-field terms are controlled separately. This does not furnish a global all-field C^p remainder bound.

Verdict:
The previous global B_{alpha,mu,p,rho} remains a legitimate abstract Banach space but is not currently justified as the matching topology. YM-MATCH-EXTRACT-1 was reformulated in a domain-indexed native activity topology.

Concrete source shapes recorded:
- CMP119 (2.18): domain-history density expansion;
- CMP119 (2.23): classical + E/R/B action decomposition;
- CMP119 (2.24): localized 1/g^2 coupling recursion;
- CMP119 (2.31): R-term pointwise exponential decay;
- CMP119 (2.42): B-term pointwise exponential decay;
- CMP122-I (1.70): large-field C-term exponential bound;
- CMP122-II (1.99)-(1.100): post-R localized remainder bounds;
- CMP122-II Theorem 1: preservation of the CMP119 inductive density class while effective couplings remain sufficiently small.

Proved:
- YM-COMPACTNESS-CORRECTION-1.
- YM-RG-CHAINRULE-1 (abstract full-R chain rule).
- YM-NATIVE-TAIL-1 (pointwise polymer decay + rooted counting => anchored norm).

Full-R audit:
E derivative identities are valid at finite volume, but E/S/L Banach mapping and localization boundedness remain open; therefore C_0 is not instantiated.

STEP_0:
Not attacked with constants because no mathematically legitimate K_0 containing the actual matching output has been constructed.
All f_0^±,H_0,A_0,B_0,C_0 remain UNKNOWN.

Result:
STATUS: BLOCKED AT YM-MATCH-EXTRACT-1.

Exact continuation point:
Build the exact native density-space predicate and rooted summability constants from CMP119/CMP122, then establish a closed/bounded K_0 at fixed small g_match. Only then attempt the first full-R STEP_0 certificate.


## 2026-09-26 — Iteration 4: mathematical validity of Balaban-native matching space

Scope:
Only YM-MATCH-EXTRACT-1 state-space validity. STEP_0 was prohibited and not attempted.

Strongest correction:
The native weighted l-infinity H-infinity coefficient product is not a state space of actual effective densities. The correct mathematical object is a nonlinear reconstruction graph over a closed linear compatibility layer.

History audit:
The full density sums over admissible domain histories. A sup over histories does not control that sum. No exact-disjointness theorem, cutoff-uniform finite-overlap constant, or complete source history weight was verified. Added YM-HISTORY-SUP-FAIL-1 and YM-HISTORY-WEIGHT-1.

Metric audit:
The source ledger identifies CMP109 d_j(X) as a shortest-tree localization size divided by M. Diameter-only exponential counting is false. For a bounded-degree block graph, connected m-block animals through an anchor are bounded by Delta^{2(m-1)} using a canonical DFS traversal. Hence d-shell counting is reduced to the Balaban-specific coercivity |X|<=a_0+a_1 d_j(X). History entropy remains separate.

Analytic-domain audit:
A raw common analytic core may collapse because historical analytic radii depend on g_j and early UV couplings approach zero. A normalized source-dependent analytic chart is required; no uniform chart/collar theorem was extracted.

Coupling audit:
The native coordinate is a localized c_j(x)=1/g_j^2(x). A scalar c alone is insufficient near domain boundaries. The augmented state must include a coupling-profile defect q.

Primary-source audit:
CMP119/CMP122 direct equation-level verification could not be upgraded: repository DOI links redirect to Springer endpoints inaccessible to the available browser. Equation-level formulas therefore retain secondary visual-transcription status. Paper-level metadata/abstracts were verified. The CMP109 d_j definition remains source-extracted through the audit ledger.

Result:
STATUS: BLOCKED BEFORE K_0.

Exact continuation:
Extract exact primary admissible-history/nesting/characteristic structure and history summation mechanism; prove Balaban-specific tree coercivity; construct normalized analytic charts with a positive fixed reference collar; then prove reconstruction continuity and native RG preservation.


## 2026-10-13 — Iteration 5: exact Balaban history and history-sum mechanism

Scope:
Only YM-HISTORY-RECON-1. STEP_0 and K_0 construction were not attempted.

Primary-source breakthrough:
Direct article PDFs for CMP119 and CMP122-II became accessible and were inspected page-by-page. History-related claims previously carried as secondary transcriptions were upgraded to direct primary verification.

History structure:
CMP119 Eq. (2.1) gives nested/interleaved Omega/Lambda localization domains. S_j lies in Omega_j intersect Lambda_j^c with source cube/collar conditions. Eq. (2.18) sums only the outer Omega/Lambda histories; the S_j sum is explicitly inside T_k.

Raw versus compressed history:
CMP119 Eqs. (3.2),(3.3),(3.16) are exact hard {0,1} partitions of unity over raw P/Q/R sets. These raw choices are not retained as final history coordinates: fixed P,Q choices producing the same Omega are resummed into zeta(Omega^c), and S is summed in T. Therefore chi_k(Omega_k) in Eq. (2.17) is not a complete-history characteristic and sum_H chi_H=1 must not be asserted at the compressed level.

T-operation:
Directly verified as an integration/operator composition with characteristic factors, RG delta constraints, gauge fixing and internal S sums; it factorizes over disjoint large-field components and into ordered one-step operations.

History entropy:
CMP122-II directly states that summations over admissible sequences are replaced by exp{O(1)(MR_j)^(-d)|Z_j|} factors. Large-field characteristic functions yield negative p_0(g_j)^2/tree-size and p_0(g_j) penalties. Eq. (1.79) combines the positive entropy and negative energy terms; the induction (1.80)-(1.88) pays the cost across scales. Eq. (1.89) yields a clean local T'_k(X)1 suppression. Thus the source does pay history entropy.

Operator majorant:
CMP122-II (1.75) gives the normalized local T'_k operator estimate. Combining (1.75) with (1.89) yields a source-local tau-type bound.

Coupling profile:
CMP119 (2.24) shows phi_j is a localization cutoff attached to Lambda_j and is not summed over. Once a deterministic Lambda->phi_Lambda convention is fixed, the localized profile is derived; q is removed as an independent physical coordinate.

New abstract theorem:
YM-HISTORY-RECON-ABSTRACT-1 proves absolute and Lipschitz reconstruction from fixed kernel weights H_0=sum tau_H and H_1=sum tau_H/w_H.

Remaining blocker:
Balaban's multiscale source estimate has not yet been transported into a single volume-independent local/relative density Banach target D with fixed history weights and fixed normalized analytic charts. A plain global sup norm of the unnormalized density is structurally wrong because the density is volume-extensive.

Result:
STATUS: BLOCKED AT YM-HISTORY-RECON-1.

Exact continuation:
Define D, extract fixed tau_H/w_H from (1.79)-(1.89), and prove H_0,H_1 uniform. Then establish an open ambient B_amb on which the native RG extends smoothly.


Iteration 5 tree-side-task closure:
Direct CMP109 printed p.257 was inspected. The source defines d_j(X) by a shortest tree graph along localization-cube edges. In d=4 every tree vertex belongs to at most 16 unit cubes, so |X|<=16(d_j(X)+1). Since the face-adjacency graph has degree 8, canonical DFS gives at most 64^{m-1} connected m-cube sets through an anchor. Therefore
# {X contains B: n<=d_j(X)<n+1}
< (64^32/63) exp((96 log 2)n).
This is crude but explicit, scale/cutoff/volume independent, and closes fixed-history polymer counting. It does not address history multiplicity; that is paid separately by CMP122-II's admissible-sequence entropy/large-field-energy mechanism.


## 2026-10-13 — Iteration 6: local/relative history norm

Scope:
Only YM-HISTORY-NORM-1. No K_0 and no STEP_0.

Adversarial result:
The previous global complete-history target H_0=sum_H tau_H is too strong. A product toy model with N independent binary hard-partition choices has 2^N complete histories with unit sup norm, while a fixed local source ratio is independent of N. The divergence is an artifact of summing global history norms before remote partition cancellation/factorization.

Target topology:
Selected the connected relative log-density/activity space with anchored norm
sup_Delta sum_{X contains Delta} exp(mu d_k(X)) ||K_X||.
Vacuum energy density is separated; no uniform norm of the raw extensive unnormalized density is requested.

Primary-source bridge:
CMP109 (0.26) directly provides volume/scale-uniform anchored summability of exp[-lambda d_j(X)] for sufficiently large lambda. Hence a pointwise source activity bound A exp[-Lambda d_j(X)] gives
||K||_{loc,mu} <= A C_geo(Lambda-mu).

CMP122-II translation:
- normalized local T' operator: (1.75);
- scalar large-field suppression: (1.89), with the source's arbitrary-region improvement adding tree-distance decay;
- connected R/history polymerization: (1.90)-(1.97);
- exponentiation to connected log-activity: (1.98);
- connected activity decay: (1.99);
- regular-sector bound: (1.100).

This yields explicit symbolic local H_0 bounds
H_{0,T}^{loc}(mu)
 <= exp[-a_T p_0(g_k)] C_geo(lambda_T-mu),
and
H_{0,R'}^{loc}(mu)
 <= exp[-p_0(g_k)] C_geo(kappa-mu),
under the source decay-reserve/small-coupling conditions.

Remote history:
At the connected log-activity level, disconnected remote components cancel exactly from the local projection. For a source ratio W(J_A), an abstract connected-cluster theorem gives termwise cancellation of source-independent remote clusters. Full Yang-Mills marked-source instantiation remains open.

New theorem:
YM-HISTORY-RECON-LOCAL-1:
anchored connected activity bounds h_0,h_1 imply
||Rec_A(F)||<=|A|h_0
and
||Rec_A(F)-Rec_A(G)||<=|A|h_1||F-G||,
uniformly in total volume.

Tree audit:
Direct CMP109 normalization confirms no missing M in dimensionless d_j. Source Eq. (0.26), not the crude DFS estimate, is the preferred summability input.

Result:
STATUS: BLOCKED AT YM-HISTORY-NORM-1.

Exact continuation:
prove a uniform local Lipschitz/analytic bound for the complete compressed outer-history/T -> connected-activity map in fixed normalized analytic charts. Global history H_0 should not be revived.
