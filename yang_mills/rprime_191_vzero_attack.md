# YM-191-COMPLEX-MAJORANT-1: V=0 attack and operator charts

Audit date: 2026-10-14. Base: 33605c14555735826ad4d120c9b07db0a79c7886.
Scope: reduced regular (T',V)->F only. The accepted downstream rprime_c1.md is unchanged. V-producer is DEFERRED because the reduced map is not closed.

## 1. Precise verdict: parameter and index-family dependence

The old chart is INVALID as an unqualified theorem for independent operators on all connected polymers with only its stated exponent conditions. An explicit counterexample is below. This does NOT mean that it fails for every choice of exponents, or for every more narrowly restricted native history fiber.

Define
mu=mu_F=mu_in=(1+beta)kappa-lambda_in,
Delta=kappa-mu=lambda_in-beta kappa.

The downstream assumptions impose C_geo(lambda_in)<infinity and mu>0. They do NOT impose Delta>0, much less geometric summability at Delta. Therefore they allow mu>=kappa, precisely a regime in which the old all-connected chart fails even at V=0.

For the actual restricted native family, admissibility/size/coupling restrictions must be retained. No all-connected counterexample is silently asserted to be a realizable Balaban history.

## 2. The old mixed norm and a sharp necessary-test criterion

Write the old norm, with its original normalization, as

||T||_old =
 max{
   sup_X ||T_X||_op/[A_T exp(-kappa d(X))],
   C_T,sum^(-1) sup_B sum_{X contains B}
       exp(mu_T d(X)) ||T_X||_op
 },
0<=mu_T<kappa.

Assume C_T,sum>0 is finite and uniform, as the old definition intended. Its finiteness requires the actual geometric/label sum at kappa-mu_T; positivity of that difference alone is insufficient.

Let P_T^(Sigma,k,Lambda) be the ACTUAL indexed T-domain family in the chosen chart, including multiplicity if there is more than one independent coordinate at the same domain. Set

C_Tgeo(t)=sup_{Sigma,k,Lambda,B}
 sum_{X in P_T: X contains B} exp[-t d(X)].

If C_T,sum is chosen to dominate A_T C_Tgeo(kappa-mu_T), then every radius-R old ball has the V=0 necessary-submap bound

||S(T)||_F
:=sup_B sum_{X contains B} exp(mu d(X)) ||T_X1||
<=R A_T C_Tgeo(Delta).                                      (N1)

This is sharp for independent kernel operators: choose in every nonempty integration/history fiber a norm-one evaluation kernel E_X preserving the external variables, E_X1=1, and put
T_X=c A_T exp[-kappa d(X)] E_X, 0<c<R.
The old norm is <=c and the target norm is
c A_T C_Tgeo(Delta), with the supremum understood over the same chart family.

Consequently, for that independent chart, a uniform bound for S on the ball holds iff C_Tgeo(Delta)<infinity, assuming the stated old envelope normalizer. This is a statement about the necessary linear submap, not the full (1.91) theorem.

Useful cases:
- mu<=mu_T: direct anchored embedding gives ||S(T)||_F<=C_T,sum||T||_old.
- Delta in a verified geometric-summability regime: (N1) proves the test.
- mu>=kappa on an unbounded all-connected family: the test fails.
- 0<Delta outside a verified regime: the sign alone settles nothing; polymer entropy must be counted.

A stricter source index family may reduce C_Tgeo. If it has finitely many disjoint components at each frozen sector, the all-connected family is an overcount, not an exact counterexample to that fiber. Uniformity still needs the supremum over all admissible sectors/scales and their geometric restrictions.

## 3. Explicit rank-one counterexample, including the anchored term

On the rescaled four-dimensional block lattice take X_n to be n consecutive unit cubes along the first coordinate axis, all containing the first cube B_0, n>=3. They are connected localization polymers. The source shortest-tree geometry gives d_n=d(X_n) growing linearly; in the closed unit-cube convention d_n=n-2 (the segment between the nearest faces of the first and last cube attains the lower bound). The proof only needs d_n>=n-2 and d_n<=n.

Let E_n f(z)=f(z,xi_n^0), where xi_n^0 is a fixed allowed hidden history/integration point in the artificial independent fiber. Thus E_n is a LOCAL rank-one map in the hidden variables, preserves the retained external z variables, has operator norm 1, and E_n1=1. For the counterexample take product hidden fibers with a fixed point; no measurable-selection theorem for native source fibers is assumed. It is not a scalar replacement for arbitrary f.

Put

T_{X_n}=c A_T exp(-kappa d_n) E_n,
T_X=0 otherwise, V=0.

Let
S_T=sum_{n>=3} exp[-(kappa-mu_T)d_n]<infinity,
c=(1/2) min{1, C_T,sum/(A_T S_T)} >0.

Then
sup_X ||T_X||/M_T(X)<=c<=1/2,
sup_B sum_{X contains B} exp(mu_T d(X))||T_X||
<=c A_T S_T<=C_T,sum/2.

Hence ||T||_old<=1/2, strictly inside every old radius-R ball with R>1.

The positive q=1,D=empty subsum of (1.91) gives

sum_{n>=3} exp(mu d_n)|T_{X_n}1|
=c A_T sum_{n>=3} exp[(mu-kappa)d_n].                       (N2)

It diverges for mu>=kappa. At equality each summand is c A_T>0; for mu>kappa summands grow. In finite boxes take the first N coordinates: the old input norm stays <=1/2 while the target norm is >=c A_T N if mu>=kappa. Thus there is no volume-independent operator bound. Shrinking any positive scalar radius or A_T does not repair this divergence.

The source formula allows D=empty for a single T domain. For nonnegative evaluation kernels and V=0 other admissible terms cannot cancel this necessary positive subsum. No assumption that all X_n are simultaneously components of a single physical large-field set is used. This is a counterexample to the UNRESTRICTED old coefficient chart; a source-restricted chart must prove its different index-family certificate instead.

Permitted downstream parameters can satisfy lambda_in<=beta kappa, while keeping C_geo(lambda_in)<infinity and 16a+theta<=mu. Therefore the former universal old-chart theorem is disproved, not merely missing a proof.

This says nothing about gaplessness or the validity of Balaban's physical operators.

## 4. The T operation: correct type and norm

Primary reference: CMP122-II pp.378--380, 387,
[article PDF](https://scispace.com/pdf/large-field-renormalization-ii-localization-exponentiation-h8b7ih93xo.pdf).

Source facts used here: (1.71) composes localized integrations, inherited T operations, restricted history sums and orientation/gauge choices. In (1.72) those sums also act on V. The real reference density is positive; (1.75) controls the normalized operator, and (1.89) bounds its real scalar mass. The following text adds tree decay for general regions.

The paper does not itself name a coefficient Banach space. The following is our typed completion of its actual integration operation, not a quotation of a source Banach theorem.

For X let Z_X be the retained local external configurations/parameters, and H_X(z) the admissible restricted INNER histories and orientation labels in (1.71). Let Xi_X(z,h) be ALL remaining integration variables in that operation, including those inside its inherited T factors, with gauge constraints imposed. Background fields reconstructed from these variables are evaluated there, not forgotten.

Define the feasible joint space
S_X={(z,h,xi): z in Z_X, h in H_X(z), xi in Xi_X(z,h)}.

Take E_X=B_b(S_X;C) with the sup norm and O_X=B_b(Z_X;C).
The integration kernel defines
T_X:E_X->O_X.

The gauge delta constraints mean that measures are taken on the constrained integration fibers; they are not bounded multipliers on an unconstrained Lebesgue space. Discrete history/orientation weights are part of the kernel measure.

For a term involving several X's, the operator must also act with all unintegrated variables and other history labels retained as parameters. Let ||T_X||_st be the supremum of those extended conditional operator norms over every compatible joint term space. For genuine conditional kernels this stable norm is bounded by the conditional total-variation norm

tau_X^TV =
 sup_z sum_{h in H_X(z)}
 ||K_X(z;h,dxi)||_TV.                                     (O1)

On bounded measurable test functions this is the natural full-integrand norm; on a smaller analytic test class it remains a sufficient upper bound. Conditional kernel extension to unused variables has the same bound. It preserves multiplication by functions of retained variables.

Thus
||T_X f||_infinity<=tau_X^TV ||f||_infinity.
The notation |T_X f|<=tau_X|f| must mean a supremum bound, or the pointwise positive-kernel inequality |T_X f|(z)<=int |f|d|K_X(z)|. It is not pointwise multiplication by a scalar T_X1.

For the positive REAL operator T_X^0, mass m_X(z)=T_X^0 1(z) controls the full kernel on all bounded integrands:
|T_X^0 f|(z)<=m_X(z)||f||_fiber.
For the source complex continuation with |sigma|<=s, the positive-reference domination gives
tau_X^TV<=e^s sup_z m_X(z);
the published normalized estimate also supplies
||T_X||_st<=e^{3s} sup_z |T_X1(z)|
on its stated source integrand regime.

For an arbitrary complex operator, |T_X1| does not bound its norm. For example H=L(ev_a-ev_b) has H1=0 and ||H||=2|L| on a two-point hidden fiber. The old chart used an OPERATOR envelope; it cannot be justified by retaining only the scalar (1.89) bound and dropping the source comparison mechanism.

Exact source domain status:
the feasible-fiber type above retains every integrated and unintegrated variable by definition. A fully enumerated gauge/old-history dictionary for the inherited (1.100)[IV] factors and every extended operator domain has NOT been extracted here. It is an explicit remaining requirement, not an invented common A_k domain. In particular T_X is not asserted to be an endomorphism of the downstream field-only algebra.

## 5. Candidate A: a stronger independent operator chart

Choose lambda_T>0 in a verified geometric-summability regime and nu_T=mu+lambda_T. Define

M_T^new(X)=epsilon_T exp[-nu_T d(X)],
M_V^new(Y)=epsilon_V exp[-nu_V d(Y)],
nu_V=mu+lambda_V, C_geo(lambda_V)<infinity.

Use ||T_X||_st (or conditional TV kernel norm), together with a weighted-sup regulator by M_T^new and an anchored norm at exponent mu. For V keep the supremum over ALL hidden variables/history labels on which its integrand depends. If labels are independent coordinates their multiplicities enter the anchored sums explicitly.

For conditional countably additive kernels, the TV norm gives a Banach space: uniform TV limits preserve countable additivity and measurability of each kernel evaluation. The weighted supremum and anchored sum constraints form a complete mixed space by coordinate limits and Fatou. These are complex Banach coefficient spaces. Their open balls have any chosen fixed positive normalized radius. They are mathematically stronger than merely shrinking the old amplitude. Already

sup_B sum_{X contains B}e^{mu d(X)} R_T M_T^new(X)
<=R_T epsilon_T C_geo(lambda_T),

so the V=0 necessary subsum is strictly <r_* if that explicit expression is <r_*.

This is NOT yet a theorem for the full (1.91) map. Joining geometry, exponential V factors, operator extensions and entropy still need the full majorant.

Physical source embedding:
UNKNOWN for this ordinary-metric independent chart. The coarse extracted exp[-kappa d(X)] bound does not imply the new nu_T envelope when nu_T>kappa. A conditional repair for genuinely size-restricted native T domains is described next. V has a separate relative-metric obstruction; source (1.68) is not automatically the displayed ordinary-metric V weight.

### 5.1 Keep size/coupling information instead of discarding it

Suppose the relevant native T index dictionary proves d(X)<=D_k, and the REAL mass satisfies
m_X<=exp[-c_0 p_0(g_k)], c_0=2/(1+beta_0).
Then the stable complex norm satisfies, with |sigma|<=s_0,

||T_X||_st
<=exp[s_0-c_0 p_0(g_k)+nu_T D_k] exp[-nu_T d(X)].             (A1)

Thus a uniform embedding follows if
epsilon_T(gamma):=
sup_{0<g_k<=gamma}
 exp[s_0-c_0 p_0(g_k)+nu_T D_k]
is sufficiently small. This is a genuine inequality, not a radius renaming.

The p.389 argument pays a size cost involving 2 kappa(100 R_k)^d from p_0(g_k). That suggests this route for the native small-component family. To instantiate (A1), the EXACT metric bound D_k, applicable T index family, coupling relation and any constants must be proved. The arbitrary-region bound after (1.89) does not impose a small-component diameter restriction on all artificial independent coordinates.

(A1) is PROVED AS A CONDITIONAL EMBEDDING LEMMA; the complete source dictionary needed to use it is OPEN.

## 6. Candidate B: structured relative-kernel chart with a genuine open radius

Fix one SOURCE kernel system at a frozen Sigma. At each X retain a positive reference measure nu_X^0 on S_X with total mass m_X(z). Write the source complex kernel as e^{sigma_X}nu_X^0, |sigma_X|<=s_0, and perturb its density by

K_X(b)=(e^{sigma_X}+b_X)nu_X^0,
b_X in B_b(S_X;C), ||b||=sup_X||b_X||_infinity.

This parameter space is Banach. For any fixed epsilon>0, ||b||<epsilon is a genuine open complex neighborhood of b=0, independent of volume, cutoff and X. T_X(b) is affine/entire in b and retains the source integration/history fibers. This is an open PARAMETER chart centered at each source operator, not a claim that the set of all physical histories/operators is open.

Its TV bound is
||T_X(b)||_st<=(e^{s_0}+epsilon)m_X.

For 0<=s_0<=1, positivity gives the independent phase-sector estimate
Re T_X(0)1 >= e^{-s_0}cos(s_0)m_X.
Indeed Re(e^sigma)=e^{Re sigma}cos(Im sigma)>=e^{-s_0}cos(s_0)>0.
Therefore
|T_X(b)1|>=[e^{-s_0}cos(s_0)-epsilon]m_X.
The fixed choice epsilon=e^{-2}/2 is uniform and positive: it is smaller than e^{-1}cos(1), which bounds the reference factor from below on 0<=s_0<=1. No uniform absolute lower bound on the tiny mass m_X is needed; the perturbation is RELATIVE to it. This proves local complex operator/normalization stability on that parameter ball. In the normalized parameter norm ||b||/epsilon, the radius-2 ball also remains nonzero because 2epsilon=e^{-2}<e^{-1}cos(1). Thus a genuine local outer radius >1 is available; this is NOT an output-image certificate.

If an action coefficient rather than a density coefficient is preferred, set
b_X=e^{sigma_X}(e^{h_X}-1).
Then ||b||<epsilon follows from
e^{s_0}(e^{||h||}-1)<epsilon,
again giving a fixed positive radius for aggregated exponent perturbations. A raw local-activity-to-h map is not silently included.

Physical source embedding:
YES at b=0 by construction, fiberwise. This does not prove an embedding of every independent operator, or uniform geometric output bounds over all centers Sigma. The old source mass envelope is preserved up to a fixed factor; a missing decay reserve is not created.

V can be varied on its full hidden-history function space using a prescribed source-compatible regulator. Such a regulator must keep the actual relative metric Z_loc(h) and label geometry, rather than replacing it by ordinary d(Y) without proof.

Candidate B is therefore a PROVED LOCAL OPEN KERNEL CHART, with the global reduced majorant still OPEN.

## 7. Relative V geometry: a concrete uncontrolled subsum

Let Z_loc(h) denote the region used in the V bound (1.68), and Ycal=union_i Y_i the fixed second-class region excluded by the regular F bound (1.97). These are not identified.

At regular X', a V domain can meet a first-class T component X_j while avoiding Ycal. Its source relative metric can then be far smaller than ordinary d(Y). Thus regularity alone does not establish
d_{k,Z_loc(h)}(Y)=d_k(Y).

The q=1,D={Y} terms joined to X give the positive subsum

sum_{X,Y: determine regular X'} 
 || T_X [ V_Y integral_0^1 exp(t V_Y)dt ] ||.

Under positive domination its majorant contains
T_X^+[M_V(Y;h,xi) exp(M_V(Y;h,xi))].

If Y traverses a large T core, its relative V cost may omit the tree size of that core. The unproved estimate is the sum over Y attached to X, weighted by e^{mu d(X')} and then summed over X and its histories. Ordinary C_geo(lambda_V) does not bound it unless a metric-conversion/size-payment lemma is proved. This is a specific UNCONTROLLED subsum for source-compatible candidate B, not a demonstrated physical divergence.

Candidate A controls such terms only by demanding a stronger ordinary V weight; its physical embedding remains unproved. No V-producer analysis is needed to expose this reduced obstruction.

## 8. Full positive majorant with labels and real operator constants

Fix the exact outer index set
I_Sigma^reg(X')={(q,selected labelled T domains,D): determine X'}.
Use the source unordered selections with multiplicity 1 per concrete selection. If selections are converted to ordered lists, insert their EXACT counting factors; do not add a q! denominator to a noncommuting ordered product without an equivalence proof.

For one I retain the joint hidden history/orientation label zeta in H_Sigma(I) and the constrained integration fiber Xi_Sigma(I,zeta). Each local V is pulled back by its actual map J_{I,Y,zeta}. On the B_b feasible-configuration completion a valid pullback is contractive:
c_{I,Y,zeta}=||J_{I,Y,zeta}||<=1.
A purported analytic-domain map with an unproved image is not assigned c=1; it is an open typing obligation.

For fixed kernel inputs, form the positive iterated JOINT measure dN_{I,zeta}^+ by replacing each local kernel by its total variation and retaining all conditional dependencies. Its discrete weights include the inherited history sums, gauge/orientation averaging weights and restrictions. The history sum must be counted exactly once; it is not a new multiplicity factor after taking the full kernel norm.

For chart B on a normalized radius-R_T ball, replace each density by
w_X^+(R_T)=e^{s_X}+epsilon R_T
relative to nu_X^0. This defines the common dominating joint measure dN_{I,zeta}^+(R_T) explicitly by iterated kernel composition. For an independent kernel chart, one instead needs its specified dominating measures, or uses the stable norm bound below.

Let m_{I,Y,zeta}(xi) be the nonnegative bound for the PULLED-BACK V coordinate on that joint fiber, including c_{I,Y,zeta} if only a local sup envelope is known. The full positive majorant is

M_{F,Sigma}^{abs}(X';R_T,R_V)
=
sum_{I in I_Sigma^reg(X')}
 integral_[0,1]^D dt
 sup_{retained external z}
 sum_{zeta in H_Sigma(I)}
 integral_{Xi_Sigma(I,zeta)}
 dN_{I,zeta}^+(R_T)
 [product_{Y in D} R_V m_{I,Y,zeta}(xi)]
 exp[sum_{Y in D} t(Y)R_V m_{I,Y,zeta}(xi)].                 (M1)

All histories, discrete multiplicities, integrals, exponential factors and restrictions are retained as actual kernel/index data in (M1). No unspecified O(1) absorbs a history count. This is an exact positive-kernel formula once those data are specified, and a nonnegative EXTENDED series. A fully enumerated source certificate with every inherited domain/weight evaluated is NOT extracted here; neither finiteness nor a numerical/quantitative uniform bound is claimed.

When stable operator bounds and compatible restriction maps have actually been proved, a coarser whole-ball majorant for candidate A is

sum_I c_I^op
 product_h [R_T M_T^new(X_h)]
 product_{Y in D}[R_V c_{I,Y} M_V^new(Y)]
 exp[sum_{Y in D}R_V c_{I,Y}M_V^new(Y)].                    (M2)

Here c_I^op is the actual norm of any additional operator/restriction bridge after the stable conditional operator norms are removed. For pure retained-variable kernel extensions on the feasible B_b completion it is <=1. Every unresolved bridge remains explicit. The outer index set still includes every concrete selection.

The desired inequality is

Q_new(R_T,R_V)=
sup_{Sigma,k,Lambda,B}
sum_{X' contains B}e^{mu d_k(X')}M_{F,Sigma}^{abs}(X';R_T,R_V)
<r_*^native=(a/9)e^{-16a-sqrt(7)theta}.                      (M3)

For the old unrestricted chart the q=1,D=empty part already violates finiteness whenever C_Tgeo(Delta)=infinity, in particular mu>=kappa. For candidate A that necessary part is controlled, but (M3) and physical embedding are OPEN. For candidate B local kernel inflation is proved, but the attached-one-V relative-geometry subsum and full history/domain entropy in (M1) are UNCONTROLLED.

No values R_up>1,r_image<r_* are asserted. No chain rule is applied.

## 9. Status and exact next step

STATUS: old universal independent-chart claim REFUTED; source-compatible reduced theorem GAP.

V=0 NECESSARY TEST:
sharp criterion C_Tgeo(lambda_in-beta kappa)<infinity;
explicit counterexample for mu>=kappa on all connected polymers;
valid in the extra-reserve regimes described in Sect.2.

OLD CHART: INVALID AS STATED WITHOUT AN EXTRA INDEX/EXPONENT CERTIFICATE.
NATIVE RESTRICTED VERSION: UNKNOWN until that certificate is instantiated.

T' OPERATOR DOMAIN/NORM:
full hidden-history/integration fibers -> retained external variables;
stable conditional TV/operator norm, not scalar weight.
Complete explicit inherited-variable dictionary remains OPEN.

NEW CHART CANDIDATES:
A stronger independent stable-kernel/operator weights;
B relative-density Banach balls, fixed epsilon=e^{-2}/2 for s_0<=1.

PHYSICAL EMBEDDING:
A UNKNOWN;
B YES at the source center in each fixed fiber, not an open physical family.

COMPLEX ABSOLUTE MAJORANT:
(M1)/(M2) specified; uniform anchored bound NOT PROVED.
UNIFORMITY IN Sigma: local chart radius YES; global output/geometry UNKNOWN.
STRICT IMAGE INCLUSION: NOT PROVED.
REDUCED UPSTREAM C^1: GAP.

EXACT FAILED INEQUALITY:
R_T A_T C_Tgeo(lambda_in-beta kappa)<infinity
fails in the unrestricted counterexample regime.
For the structured replacement the unproved inequality is (M3), already at the attached-one-V subsum.

NEW BLOCKER:
YM-191-T-STABLE-DOMAIN-1 + YM-191-RELATIVE-GEOMETRY-1.
Do not resurrect the old chart by shrinking one scalar radius.

EXACT NEXT STEP:
For the regular native T-family, extract the exact size/coupling bound d(X)<=D_k and its history multiplicity; test (A1). In the SAME hidden-variable fiber, prove the q=1,D={Y} attached-domain sum with relative V weights and output exponent mu. If this fails or remains unproved, stop before the V-producer. Only after that may the full q,D entropy and whole-ball image be closed.

No K_0, STEP_0, boundary B', physical source derivatives or other RG sectors.
