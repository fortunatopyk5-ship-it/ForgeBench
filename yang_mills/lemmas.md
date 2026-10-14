# Lemmas

## LEMMA ID: YM-BRIDGE-1
Statement:
Let (H,Omega,Hamiltonian H) be an OS-reconstructed theory with H>=0. Suppose a centered local gauge-invariant algebra produces a dense set D in Omega^perp. Assume there is a common m_*>0 such that for every psi in D there is C_psi<infinity with
<psi,e^{-tH}psi> <= C_psi e^{-m_* t}
for all sufficiently large t.
Then Spec(H)∩(0,m_*)=empty.

Purpose:
Turn Euclidean-time clustering into the spectral mass gap without treating that implication as automatic.

Dependencies:
OS reconstruction; density/cyclicity of the chosen centered algebra.

Status:
PROVED.

Proof:
For psi, spectral theorem gives
<psi,e^{-tH}psi> = integral exp(-tE) dmu_psi(E).
If mu_psi([0,m_*-epsilon])>0 for some epsilon>0, the LHS is at least
mu_psi([0,m_*-epsilon]) exp(-(m_*-epsilon)t),
contradicting the assumed upper bound as t->infinity.
Thus mu_psi([0,m_*))=0 for all psi in D. The spectral projection
P=1_[0,m_*)(H) is bounded and vanishes on dense D subset Omega^perp, hence P|Omega^perp=0.
Centering removes the vacuum atom at E=0.

Gap:
Need the hypotheses from an actual continuum Yang–Mills construction.

Counterexample search:
Spatial clustering without OS/time-translation reconstruction is not sufficient; the proof specifically uses a positive Euclidean-time semigroup.

---

## LEMMA ID: YM-CUTOFF-BRIDGE-1
Statement:
Let a_k->0 be reflection-positive lattice theories whose renormalized gauge-invariant Schwinger functions converge to an OS-reconstructible limit. If for a dense generating family the diagonal Euclidean-time correlators have a common lattice-unit decay exponent gamma(a_k) with
liminf gamma(a_k)/a_k >= m_*>0,
and the correlators converge with their t=0 norms, then the continuum Hamiltonian has gap at least m_*.

Purpose:
State the exact cutoff scaling needed from a lattice estimate.

Dependencies:
reflection positivity, convergence of renormalized correlators, YM-BRIDGE-1.

Status:
PROVED under the stated convergence/reflection-positivity hypotheses.

Proof:
At cutoff a_k (and after a volume-uniform thermodynamic estimate), reflection positivity gives a transfer Hamiltonian H_k. The common lattice-unit exponent gamma(a_k) excludes transfer spectrum below gamma(a_k), so if gamma(a_k)/a_k >= m_* then for every centered local vector psi_k,
C_k(t_k)=<psi_k,exp(-t_k H_k)psi_k> <= exp(-m_* t_k) C_k(0)
at lattice times t_k=n_k a_k.
For fixed physical t choose n_k=floor(t/a_k), hence t_k->t. By convergence of the renormalized diagonal Schwinger functions and their t=0 norms,
C(t) <= exp(-m_* t) C(0).
OS reconstruction identifies C(t)=<psi,exp(-tH)psi> in the continuum. YM-BRIDGE-1 then excludes continuum spectrum in (0,m_*), provided these local vectors are dense in Omega^perp.

Gap:
The theorem is conditional on the actual Yang-Mills hypotheses: cutoff/volume-uniform gamma(a)>=m_* a, convergence of renormalized local correlators including norms, and OS reconstruction. Those are not proved by this bridge.

Counterexample search:
If only gamma(a)>0 is known, gamma(a)/a may tend to zero. Fixed-cutoff positivity therefore does not imply a continuum mass gap.

---

## LEMMA ID: YM-IR-1
Statement:
Along a continuum Wilson trajectory beta=beta(a)->infinity, prove for some c_G>0
gamma(a) >= c_G a Lambda_YM
uniformly in volume, where gamma(a) is a common Euclidean-time exponential clustering rate in lattice units for a dense gauge-invariant local algebra.

Purpose:
This is the minimal IR estimate needed after continuum construction.

Dependencies:
none beyond the regulated lattice theory and a chosen continuum trajectory.

Status:
CONJECTURED.

Proof:
None.

Gap:
Weak-coupling, volume-uniform, cutoff-sharp correlation decay.

Counterexample search:
Any estimate gamma(a)>=exp(-C beta) is insufficient unless C and logarithmic/power corrections are quantitatively sharp relative to the RG scaling of a Lambda_YM.

---

## LEMMA ID: YM-FI-1
Statement:
For a local reversible stochastic-quantization generator L_a with invariant Wilson measure, let lambda_a be a Poincare/spectral relaxation rate and v_a a finite-range derivative-propagation scale obtained from commutators [nabla_e,L_a]. Prove a normalization-independent bound
lambda_a/v_a >= c_G a Lambda_YM
along the continuum trajectory, together with uniform locality constants sufficient to convert this ratio into Euclidean covariance decay.

Purpose:
A more attackable functional-inequality proxy for YM-IR-1.

Dependencies:
A Shen–Zhu–Zhu/Guionnet–Zegarlinski type semigroup-to-spatial-mixing theorem with cutoff-explicit constants.

Status:
CONJECTURED.

Proof:
At strong coupling an analogous mechanism is rigorous. The ratio is invariant under an overall rescaling L_a -> r_a L_a, unlike lambda_a alone.

Gap:
No weak-coupling lower bound with the required physical scaling is known here.

Counterexample search:
Bakry–Emery curvature gives no continuation: for SU(N) in d dimensions the known strong-coupling constant is
K_S=N(1/2-8|beta|(d-1)),
so in d=4 it is positive only for |beta|<1/48 and becomes negative in the continuum regime beta->infinity.

---

## LEMMA ID: YM-UV-1
Statement:
Construct a nontrivial continuum limit of 4D Wilson lattice Yang–Mills along an asymptotically-free trajectory, with renormalized gauge-invariant Schwinger functions satisfying corrected Osterwalder–Schrader hypotheses strongly enough for Wightman reconstruction.

Purpose:
Existence half of the Millennium problem.

Dependencies:
constructive RG, UV stability, thermodynamic limit, observable renormalization, Euclidean covariance restoration.

Status:
UNKNOWN.

Proof:
None in this notebook.

Gap:
Balaban-type UV control does not by itself supply the full R^4 OS theory with the IR limit and mass gap.

Counterexample search:
A subsequential or fixed-volume UV limit is insufficient if thermodynamic-limit control or nontriviality is missing.


---

## LEMMA ID: YM-SEMIGROUP-1
Statement:
Let mu be a Gibbs measure on a bounded-degree lattice, invariant for a reversible diffusion semigroup P_t with generator L. Assume:

(1) Poincare decay:
Var_mu(P_t f) <= exp(-2 lambda t) Var_mu(f).

(2) Diffusion/carre-du-champ identity:
P_t(fg)-P_t f P_t g
= 2 integral_0^t P_s Gamma(P_{t-s}f,P_{t-s}g) ds,
with Gamma decomposing into local edge gradients.

(3) Weighted gradient propagation: for every kappa>0 in some interval and local f supported on A,
G_e(P_t f) <= L_f exp(v_kappa t-kappa d(e,A)),
where G_e is the local gradient norm and v_kappa is independent of volume.

Then for local f,g with supports A,B and R=d(A,B),
|Cov_mu(f,g)| <= C_{f,g,kappa} exp(-gamma R),
with a common rate
gamma >= kappa lambda/[2(lambda+v_kappa)]
(up to an arbitrarily small loss if the underlying graph only has polynomial volume growth).

If the Euclidean lattice measure is reflection positive, the physical transfer-Hamiltonian gap in lattice units is at least this common clustering rate on the dense local gauge-invariant sector.

Purpose:
Separate the two ingredients hidden inside “functional inequality implies mass gap”: relaxation in auxiliary Markov time and finite propagation in Euclidean space.

Dependencies:
Reversible diffusion calculus; reflection positivity only for the last spectral interpretation.

Status:
PROVED under the stated abstract hypotheses.

Proof:
By invariance,
Cov(f,g)=mu(P_t(fg)-P_t f P_t g)+Cov(P_t f,P_t g).
The Poincare assumption and Cauchy-Schwarz give
|Cov(P_t f,P_t g)| <= exp(-2 lambda t) ||f-mu f||_2 ||g-mu g||_2.
For the first term, the diffusion identity and weighted gradient bound give
||P_t(fg)-P_t f P_t g||_infty
<= C L_f L_g exp(-kappa R/2+2v_kappa t).
Here one uses
sum_e exp[-kappa(d(e,A)+d(e,B))]
<= C_{kappa,d}|A||B| exp(-kappa R/2)
on a polynomial-growth lattice. Choosing
t = kappa R/[4(lambda+v_kappa)]
makes both exponents at least
kappa lambda R/[2(lambda+v_kappa)].
This proves the claim.

Gap:
At weak coupling, no required cutoff-sharp lower bound on lambda/(lambda+v_kappa) has been proved here.

Counterexample search:
A global Poincare constant may be unnecessarily strong and may be degraded by slow topological/global modes even if local physical correlations are short-ranged. Therefore the next version should seek a conditional/local or observable-restricted replacement.

---

## LEMMA ID: YM-RG-LANDING-1
Statement:
Fix a block factor b>1. Suppose an exact gauge-invariant RG can be iterated to a scale k_*(a) such that the coarse spacing
a_* = b^{k_*(a)} a
obeys c_1/Lambda_YM <= a_* <= c_2/Lambda_YM, and the exact effective measure admits a polymer representation with activities z(X) satisfying a volume- and cutoff-independent Kotecky-Preiss type bound
sup_B sum_{X contains B} ||z(X)|| exp(alpha |X| + mu diam(X)) <= epsilon < epsilon_KP
for fixed alpha,mu>0. Assume the exact RG map sends microscopic local gauge-invariant observables to quasi-local coarse observables whose tails decay faster than the same polymer rate.

Then the original lattice theory has exponential gauge-invariant correlation decay with physical rate at least c Lambda_YM, and, using reflection positivity at the ORIGINAL Wilson lattice level, its transfer Hamiltonian has a physical gap >= c Lambda_YM.

Purpose:
Turn the vague slogan “RG flows to strong coupling” into one precise landing inequality in a known mixing domain.

Dependencies:
Standard convergent polymer/cluster expansion once the KP bound holds; exact pullback of observables; YM-BRIDGE-1 / lattice transfer spectral argument.

Status:
PARTIALLY PROVED.

Proof:
The KP bound yields exponential connected-correlation decay exp(-mu R_*) in coarse lattice units for quasi-local observables. Pulling the estimate back through the exact RG gives microscopic decay exp[-c mu r/a_*]. Since a_* is comparable to Lambda_YM^{-1}, the physical decay exponent is >= c' Lambda_YM. Reflection positivity is required only for the original Wilson measure; the blocked effective action itself need not be reflection positive. The spectral theorem then gives the transfer gap.

Gap:
The landing hypothesis itself: prove that 4D asymptotically-free Yang-Mills enters such a uniform polymer domain at a physical scale O(Lambda_YM^{-1}). This is precisely the UV-to-IR crossover not controlled by present perturbative/strong-coupling estimates.

Counterexample search:
Merely reaching an O(1) effective coupling is not enough; the KP norm must actually be below its convergence threshold with cutoff-independent constants.


---

## LEMMA ID: YM-CROSSOVER-1
Statement:
Fix a block factor b>1 and a Banach space B of gauge-invariant effective interactions with an exact RG map R. Assume the UV construction supplies, at the first matching scale where a chosen renormalized coupling reaches a fixed small value g_match>0, a cutoff- and volume-uniform closed and bounded matching tube K_match subset B of possible effective actions, with matching spacing a_match comparable (up to a fixed G-dependent factor) to Lambda_YM^{-1}.

Prove that there exist an integer M_G<infinity and q<1, both independent of the original cutoff a and volume, such that
R^{M_G}(K_match) subset D_KP(q),
where D_KP(q) is a rigorously specified strong-mixing/polymer domain satisfying a Kotecky-Preiss exponential norm bound.

Purpose:
Reduce the nonperturbative UV-to-IR crossover to a FINITE number of RG steps independent of the UV cutoff. Once this inclusion holds, YM-RG-LANDING-1 follows.

Dependencies:
A UV RG package that reaches K_match uniformly and an exact RG map with controlled observable pullback.

Status:
CONJECTURED.

Proof:
No proof of the inclusion. The reduction from this inclusion to a gap is proved by YM-RG-LANDING-1.

Why finite M is plausible but not proved:
The cutoff-dependent number of RG steps occurs only while running from g_0(a)->0 to the fixed g_match. After g_match is fixed, going to any other fixed strong-coupling threshold spans a fixed logarithmic scale interval, hence a fixed number of b-blocks in the RG heuristic. Establishing the corresponding nonperturbative inclusion in the full effective-action space is exactly the missing step.

Gap:
Construct a concrete B, K_match and D_KP and validate the finite-step inclusion without projecting away relevant/irrelevant interactions or assuming monotonic coupling flow.

Counterexample search:
Tracking a single scalar coupling is insufficient: irrelevant operators generated by RG can leave the strong-mixing domain even when a nominal effective coupling is “large”. The lemma must control the full interaction norm.


---

## LEMMA ID: YM-KP-ANCHOR-1

Statement:
Let P be a hard-core polymer gas on finite connected block polymers with incompatibility given by overlap. Define

Q_{alpha,mu}(z)
 = sup_B sum_{X contains B} |z(X)| exp(alpha |X| + mu diam(X)).

If alpha,mu>0 and Q_{alpha,mu}(z) <= q < alpha, then the Kotecky-Preiss criterion holds with
a(X)=alpha |X|,
g(X)=mu diam(X).
Hence the cluster expansion converges absolutely uniformly in volume. Moreover the generalized KP tail bound has the exponential weight exp(sum g(X_i)); consequently source-connected clusters spanning supports at block distance R are bounded by C exp(-mu' R) for every fixed mu'<mu (with C depending on the fixed source supports but not total volume).

Purpose:
Give a concrete numerical threshold q_KP=alpha for the landing domain used in YM-CROSSOVER-1.

Status:
PROVED, using the standard Kotecky-Preiss theorem.

Proof:
For any polymer X0,
sum_{X incompatible with X0}
 |z(X)| exp(alpha|X|+mu diam(X))
<= sum_{B in X0} sum_{X contains B} ...
<= |X0| q
< alpha |X0| = a(X0).
This is the KP hypothesis. The weighted cluster tail statement is the second conclusion of the generalized KP theorem. A connected source cluster spanning distance R must spend total diameter at least R up to fixed source-collar corrections; reserve an arbitrarily small part of mu for those corrections.

Gap:
To apply this to YM, one must construct the scalar super-polymer activities z_Phi from the exact effective interaction and prove Q_{alpha,mu}(z_Phi)<alpha.

Counterexample search:
Failure of this sufficient KP bound does NOT imply absence of a mass gap; D_KP is a strict sufficient domain, not a characterization of all gapped theories.

---

## LEMMA ID: YM-TUBE-1

Statement:
Let beta:B->R be a bounded coordinate and r:B->[0,infinity) a remainder norm. Let
K_j={Phi: beta(Phi) in I_j=[beta_j^-,beta_j^+], r(Phi)<=R_j}.
Suppose for each j=0,...,M-1 there are rigorous enclosure functions F_j^-,F_j^+,G_j such that for all Phi in K_j,

F_j^-(beta(Phi),r(Phi))
 <= beta(R Phi)
 <= F_j^+(beta(Phi),r(Phi)),

r(R Phi) <= G_j(beta(Phi),r(Phi)),

and

inf_{I_j x [0,R_j]} F_j^- >= beta_{j+1}^-,
sup_{I_j x [0,R_j]} F_j^+ <= beta_{j+1}^+,
sup_{I_j x [0,R_j]} G_j <= R_{j+1}.

Then R(K_j) subset K_{j+1} for every j and therefore
R^M(K_0) subset K_M.

If in addition
sup_{Phi in K_M} Q_{alpha_KP,mu_KP}(z_Phi) <= q < alpha_KP,
then
R^M(K_0) subset D_KP(alpha_KP,mu_KP,q).

Purpose:
Turn YM-CROSSOVER-1 into a finite list of interval inequalities.

Status:
PROVED.

Proof:
Immediate induction on j using the three enclosure inequalities. The final inclusion is the definition of D_KP plus YM-KP-ANCHOR-1.

Gap:
The Yang-Mills one-step functions F_j^±,G_j and the final Q bound are not known.

Validated numerics:
scripts/verify_crossover_tube.py checks this induction with exact rational interval arithmetic once rigorous polynomial enclosure formulae are supplied.

---

## LEMMA ID: YM-RG-DRIFT-1

Statement:
Let
T={Phi: beta_sc <= beta(Phi) <= beta_max, r(Phi)<=R}.
Assume constants delta>0, 0<=rho<1 and B>=0 satisfy, for every Phi in T,

beta(R Phi) <= beta(Phi)-delta,

r(R Phi) <= rho r(Phi)+B,

B <= (1-rho)R.

Assume also that every Phi with beta(Phi)<=beta_sc and r(Phi)<=R belongs to D_KP.

Then any orbit starting in T reaches D_KP after at most

M <= ceil((beta_max-beta_sc)/delta)+1

steps, independently of the original UV cutoff and volume.

Purpose:
Remove the heuristic assumption M_G<infinity. Finite M follows from a uniform drift plus an invariant remainder tube.

Status:
PROVED.

Proof:
The remainder estimate and B<=(1-rho)R imply r_k<=R by induction. While beta_k>beta_sc, beta_{k+1}<=beta_k-delta. Therefore after at most the stated number of steps beta<=beta_sc. The final hypothesis gives membership in D_KP.

Gap:
The uniform positive drift delta and invariant-tube bounds are precisely the nonperturbative Yang-Mills estimates still missing.

Counterexample search:
Without an invariant remainder bound, scalar beta drift is useless; generated operators can grow and prevent KP landing.

---

## LEMMA ID: YM-CROSSOVER-TOY-OBSTRUCTION-1

Statement:
Monotone flow of a single coupling coordinate to "strong coupling" does not imply entry into any small-activity/cluster-expansion domain.

Status:
PROVED.

Proof / counterexamples:
1. R(g,r)=(g+1,r+1). Then g_k->infinity while r_k->infinity, so no domain r<epsilon is ever entered after finitely many steps.
2. Even with an irrelevant contraction,
   R(g,r)=(g+delta, rho r+h), 0<rho<1.
   If h/(1-rho)>epsilon, then r_k approaches a fixed value above epsilon. Thus contraction of the old remainder is not enough; the source term generated at each RG step must itself become small in the chosen strong-coupling coordinates.

Consequence:
YM-CROSSOVER-1 needs coupled inequalities for relevant and generated polymer/irrelevant coordinates, not only a beta function.

---

## LEMMA ID: YM-MATCH-EXTRACT-1

Statement (corrected Iteration 3):
At a fixed small matching coupling, extract from the Balaban-type UV density representation a cutoff- and volume-uniform CLOSED AND BOUNDED matching tube K_0 in a source-faithful domain-indexed activity topology. The matching datum must retain the admissible domain-history labels and separate E/R/B/large-field activities rather than first forcing them into one globally C^p interaction.

Required quantitative outputs:
(1) a coupling coordinate c(g) in a fixed interval I_0, with an explicit dictionary to the coefficient of the Wilson/classical-action direction;
(2) finite anchored activity norms for the E/R/B/large-field components, obtained from their native pointwise-decay/analyticity estimates plus a rooted tail-counting lemma;
(3) constants uniform in UV cutoff and physical volume over the selected matching scale.

Optional stronger conclusion:
If a separate embedding theorem maps this native matching space boundedly into B_{alpha,mu,p,rho}, then one may recover a global tube there. Such an embedding is NOT assumed.

Purpose:
Construct a mathematically legitimate K_0 before attempting STEP_0.

Status:
BLOCKED / REFORMULATED.

Known input:
Balaban's RG papers provide a domain-decomposed density representation, localized E/R/B terms, small-field analytic domains, and large-field/R-operation bounds in their native variables under small-coupling hypotheses.

Gap:
No theorem has been established here converting these estimates to the previous all-field global C^p remainder norm. The corrected native norm itself still needs a source-exact parameter dictionary and rooted summability constants.

Counterexample/adversarial issue:
Measure/activity suppression of large-field sectors does not imply pointwise smallness under a supremum over all gauge configurations. Sharp characteristic/domain factors cannot simply be differentiated in the old C^p norm.

---

## LEMMA ID: YM-RG-SOURCE-1

Statement:
For the exact RG used in YM-CROSSOVER-1, introduce local gauge-invariant sources J_F,J_G coupled to microscopic observables F,G. Along every eliminated shell, the first source derivatives of the effective action must remain quasi-local and the mixed second derivative must satisfy a shell-local bound whose sum over scales is exponentially decaying in the final block distance.

A sufficient schematic form is:
for sources A,B separated by R final blocks,

sum_{j=0}^{M-1}
 || d^2 S_j^eff /(dJ_A dJ_B) ||_{J=0}
 <= C_{A,B} exp(-nu R),

and the first derivatives admit quasi-local decompositions with the same or stronger exponent.

Purpose:
Transfer final coarse KP mixing back to correlations of the original microscopic local observables.

Status:
UNKNOWN.

Why necessary:
Exact RG gives the identity
Cov(F,G)
= Cov(E[F|V],E[G|V])
  + E[Cov(F,G|V)].
D_KP for the coarse measure controls only the first term. The second term is an independent conditional-covariance contribution unless source transport is controlled.

Counterexample:
Take visible coarse variables V with product measure and independent hidden variables H with long-range correlations, and let the RG discard H. The coarse measure is exactly in D_KP but H-observables remain long-range correlated.

Assessment:
This is a correction to the previous YM-RG-LANDING-1 statement. Measure landing alone is insufficient.

---

## LEMMA ID: YM-CUTOFF-BRIDGE-2

Statement:
Let a_k->0. After a thermodynamic limit at each cutoff (or a joint limit with the same uniform estimates), suppose reflection-positive lattice theories have physical transfer Hamiltonians H_k with

Spec(H_k)|_{Omega_k^perp} subset [m_k,infinity),
liminf m_k >= m_*>0.

For every member of a continuum-dense centered gauge-invariant local family, suppose renormalized positive-time diagonal correlators converge:

C_k(t)=<psi_k,e^{-tH_k}psi_k> -> C(t)
for every t>0,

and the limiting Schwinger functions are reflection positive and OS-reconstruct to
C(t)=<psi,e^{-tH}psi>.

Then the continuum Hamiltonian has no spectrum in (0,m_*).

No convergence of equal-time norms C_k(0) is required.

Status:
PROVED under the stated assumptions.

Proof:
Fix any m<m_*. For k large, m_k>=m. Hence for lattice times approximating t,s>0,

C_k(t+s) <= exp(-m s) C_k(t),

because the spectral measure of psi_k is supported in [m,infinity). Pass to the correlation-function limit:

C(t+s) <= exp(-m s) C(t).

If the continuum spectral measure of psi had positive mass in [0,m-epsilon], iterating this inequality in s would contradict the lower bound from that mass. Thus the continuum spectral measure has no support below m. Let m increase to m_* and use density of the centered local family.

Why this corrects YM-CUTOFF-BRIDGE-1:
A cluster-expansion prefactor C_O(a) may diverge as a->0. That does not matter once one first converts fixed-cutoff common exponential decay into an actual transfer spectral gap. The support inequality above survives the limit and avoids any assumption about C_k(0).

Additional assumptions made explicit:
- the fixed-cutoff clustering estimate must be common on a dense lattice local sector so that it is a true transfer gap, not an observable-specific mass;
- thermodynamic-limit control must precede or be uniform with the cutoff limit;
- limiting positive-time correlators must exist;
- reflection positivity is closed under the relevant Schwinger-function limit;
- the limiting centered local gauge-invariant states must be dense in Omega^perp;
- uniqueness of the reconstructed vacuum (or an explicit choice of vacuum sector) is required for the Clay-type statement.


---

## LEMMA ID: YM-RG-DERIV-1

Statement:
Let Q(V|U)>=0 be a normalized exact block kernel and let S_Phi(U) be a finite-volume fine action depending affinely on an interaction parameter Phi. Define the unnormalized effective action, modulo V-independent constants,

E(Phi)(V)
 = -log int Q(V|U) exp[-S_Phi(U)] dU.

For perturbations A,B of the fine action,

D E(Phi)[A](V)
 = E_{Phi,V}[A(U)],

D^2 E(Phi)[A,B](V)
 = -Cov_{Phi,V}(A(U),B(U)),

where E_{Phi,V} is expectation in the fine conditional measure with density proportional to
Q(V|U) exp[-S_Phi(U)].

Status:
PROVED.

Proof:
Differentiate the logarithm of the fiber partition function. The first derivative is the conditional expectation. Differentiating that expectation once more gives minus the connected conditional second moment. Additive V-independent normalization terms can be removed by the fixed localization/zero-mean convention.

Purpose:
Translate Frechet derivative estimates for one exact RG step into conditional moment/covariance estimates of one RG shell.

Important interpretation:
This does not assume the physical Yang-Mills mass gap. The covariance is in the constrained single-shell fluctuation measure conditioned on the retained coarse field V. To use the lemma one still needs quantitative conditional localization bounds uniform over the crossover tube.

---

## LEMMA ID: YM-RG-TAYLOR-1

Statement:
Let B be a Banach space, W in B, and ell in B* with ell(W)=1. Set
P=W ell, Q=I-P.
Write any Phi in a tube as
Phi=beta W+eta,
ell(eta)=0,
||eta||<=r.

Let R:B->B be C^2 on every segment beta W+t eta in the tube.
Define the center quantities

f(beta)=ell(R(beta W)),
h(beta)=||Q R(beta W)||,

and bounds

a(beta,r) >= || ell o D R_{beta W} o Q ||,
b(beta,r) >= || Q o D R_{beta W} o Q ||,
c_2(beta,r) >= sup_{0<=t<=1} ||D^2 R_{beta W+t eta}||

uniformly over ||eta||<=r.

Then

| beta(R Phi)-f(beta) |
 <= a(beta,r) r
    + (1/2)||ell|| c_2(beta,r) r^2,

and

r(R Phi)
 <= h(beta)
    + b(beta,r) r
    + (1/2)||Q|| c_2(beta,r) r^2.

Status:
PROVED.

Proof:
Second-order Banach-space Taylor formula with integral remainder, followed by ell and Q.

Purpose:
Reduce each infinite-dimensional tube inclusion to a finite collection of scalar interval bounds:
(1) center flow f,
(2) generated center remainder h,
(3) relevant/irrelevant mixing a,
(4) irrelevant amplification b,
(5) second derivative/conditional covariance c_2.

Combined with YM-RG-DERIV-1, c_2 can be attacked through conditional covariance estimates in one RG shell.

Finite enclosure form:
For beta in I_j and r<=R_j define rigorous numbers
f_j^- <= f(beta) <= f_j^+,
H_j >= h(beta),
A_j >= a(beta,R_j),
B_j >= b(beta,R_j),
C_j >= c_2(beta,R_j).
Then it suffices to choose

beta_{j+1}^-
 <= f_j^- - A_j R_j - (1/2)||ell|| C_j R_j^2,

beta_{j+1}^+
 >= f_j^+ + A_j R_j + (1/2)||ell|| C_j R_j^2,

R_{j+1}
 >= H_j + B_j R_j + (1/2)||Q|| C_j R_j^2.

This is a finite explicit list of inequalities for each crossover step.

Gap:
No rigorous Yang-Mills values for f_j^±,H_j,A_j,B_j,C_j across the intermediate-coupling region have been established here.


---

## LEMMA ID: YM-KP-NOT-NECESSARY-1

Statement:
Membership in the particular product-Haar Kotecky-Preiss domain D_KP of crossover_spec.md is not necessary for exponential clustering.

Status:
PROVED.

Proof:
Take independent block variables with probability density proportional to exp[-V(U_B)] relative to product Haar, where V is an arbitrarily large bounded one-block potential. Distinct blocks are independent, hence every connected correlation between disjoint block observables is exactly zero. But the naive one-block activity z_B=exp[-V]-1 can have arbitrarily large sup norm, so the fixed anchored condition Q_{alpha,mu}<alpha can fail arbitrarily badly.

Consequence:
YM-CROSSOVER-1 is a sufficient strong-mixing route, not an equivalent reformulation of the mass gap. If its inclusion cannot be proved, this may reflect a poor polymer chart rather than gaplessness.

---

## LEMMA ID: YM-RG-DRIFT-2

Statement:
Let
T={Phi: beta_min <= beta(Phi) <= beta_max, r(Phi)<=R}
and let beta_min < beta_sc < beta_max.
Assume, whenever beta(Phi) in [beta_sc,beta_max],

(1) delta <= beta(Phi)-beta(R Phi) <= Delta
for constants 0<delta<=Delta;

(2) r(R Phi)<=rho r(Phi)+B,
0<=rho<1,
B<=(1-rho)R;

(3) no terminal overshoot:
if beta(Phi) in [beta_sc,beta_sc+Delta], then
beta(R Phi)>=beta_min;

(4) terminal inclusion:
{Phi: beta_min<=beta(Phi)<=beta_sc, r(Phi)<=R}
subset D_KP.

Then every orbit starting in T reaches D_KP in at most
ceil((beta_max-beta_sc)/delta)+1
steps, provided it remains in the beta interval before terminal entry.

Status:
PROVED.

Proof:
The remainder tube is invariant by (2). While beta>beta_sc, (1) decreases beta by at least delta, so after the stated number of steps it crosses beta_sc. The upper decrement bound together with (3) puts the first crossed point in the terminal strip, and (4) gives D_KP.

Purpose:
Replace the unrealistically strong terminal hypothesis in YM-RG-DRIFT-1 by a bounded terminal strip and an explicit no-overshoot condition.

---

## LEMMA ID: YM-UV-SOURCE-MATCH-1

Statement:
At the matching scale, the UV RG package must map every observable F in a chosen bounded local gauge-invariant generating algebra to a quasi-local source functional

F_match = sum_X F_X

with a common exponential localization exponent nu_UV>0 and norm
sup_B sum_{X contains B} e^{nu_UV diam(X)+alpha_UV|X|} ||F_X||_{p,rho}
<= C_F,

uniformly in UV cutoff and volume. Mixed source terms generated by eliminating UV shells must obey an analogous connected bound.

Status:
UNKNOWN.

Purpose:
Close the gap between microscopic observables and the source-dependent crossover problem. Without this, YM-RG-SOURCE-1 beginning only at K_match does not control the full original correlation function.

Known support:
Balaban's UV work provides localized effective-action/cluster-expansion machinery in the small-field regime, but the specific observable-source theorem above has not been verified here from those papers.

Gap:
Extract or prove source estimates with constants compatible with the matching Banach norm.


---

## LEMMA ID: YM-COMPACTNESS-CORRECTION-1

Statement:
Let B be an infinite-dimensional Banach space, beta in B* continuous, Q:B->B bounded, and
K={Phi in B: beta_-<=beta(Phi)<=beta_+, ||Q Phi||<=R}.
Then K is closed and bounded in the directions controlled by beta and Q (and is a bounded subset when beta together with Q controls the full norm as in Phi=beta(Phi)W+QPhi). In general K need not be compact.

For the tube-induction lemmas, compactness is unnecessary: all hypotheses may be formulated with non-attained suprema and explicit finite upper bounds.

Status:
PROVED.

Proof:
Continuity of beta and Q makes the inverse images defining K closed. In the decomposition Phi=beta(Phi)W+QPhi with beta(W)=1 and Q=I-W beta, the interval and Q-radius imply
||Phi|| <= max(|beta_-|,|beta_+|)||W||+R.
Noncompactness is generic: the closed unit ball of an infinite-dimensional normed space is not compact. YM-TUBE-1 only uses universal inequalities for all Phi in K; no maximizing Phi is required.

---

## LEMMA ID: YM-RG-CHAINRULE-1

Statement:
Let B0,B1,B2,B3 be Banach spaces,
E:B0->B1 be C^2,
S:B1->B2 bounded linear,
L:B2->B3 be C^2,
and R=L o S o E.
Then

D R_Phi
 = D L_{S E(Phi)} o S o D E_Phi,

and

D^2 R_Phi[A,B]
 = D^2 L_{S E(Phi)}
     [S D E_Phi[A], S D E_Phi[B]]
   + D L_{S E(Phi)}
     [S D^2 E_Phi[A,B]].

If L is bounded linear, the first term vanishes and
D R=L S D E,
D^2 R=L S D^2 E.

Status:
PROVED.

Proof:
Standard first- and second-order Frechet chain rule.

Purpose:
Prevent the exact fiber-integration covariance identity from being incorrectly promoted to a derivative formula for the full RG map.

Yang-Mills instantiation status:
UNKNOWN. Mapping/boundedness/differentiability of E,S,L in the corrected matching topology have not been established. In particular the localization/extraction operator L is not yet a proved bounded linear operator.

---

## LEMMA ID: YM-NATIVE-TAIL-1

Statement:
Suppose a domain-indexed polymer family F_{Sigma,X} satisfies
||F_{Sigma,X}|| <= A exp(-kappa d_Sigma(X))
and there are volume-uniform constants C_count,c_count such that for every Sigma, anchor block B and n>=0,

#{X contains B: n<=d_Sigma(X)<n+1}
 <= C_count exp(c_count n).

Then for every kappa'<kappa-c_count,

sup_{Sigma,B}
sum_{X contains B}
e^{kappa' d_Sigma(X)}
||F_{Sigma,X}||
<=
A C_count /
(1-exp[-(kappa-kappa'-c_count)]).

Status:
PROVED.

Proof:
Group polymers into integer distance shells and sum the resulting geometric series.

Purpose:
This is the missing elementary conversion between Balaban-style pointwise polymer decay and a volume-uniform anchored activity Banach norm. The nontrivial source-specific input is the rooted counting estimate and the exact comparison between Balaban's d_j and the chosen polymer index.

---

## LEMMA ID: YM-GLOBAL-CP-MATCH-IMPLICATION-1

Statement:
The currently extracted Balaban estimates do NOT establish the implication

Balaban UV/small-coupling output
=>
||Q Phi_match||_{alpha,mu,p,rho} <= epsilon

for the old global all-field C^p polymer norm.

Status:
PROVED AS A SOURCE/LOGICAL NON-IMPLICATION; NOT a theorem that such an embedding is impossible.

Reason:
The extracted hypotheses and conclusions control:
- analytic local activities on restricted regularity domains;
- sector/domain-history dependent E/R/B decompositions;
- separate large-field activities and suppression factors.

They contain no bound on derivatives of a single globally recombined interaction over all U in G^{E(X^+)} and no bounded embedding theorem into B_{alpha,mu,p,rho}. Therefore using them to infer the global norm estimate is an unsupported strengthening.

Important limitation:
It remains possible that a different argument proves the exact recombined density is globally smooth and bounded in such a norm. That theorem is simply absent from the currently extracted UV package.


---

## LEMMA ID: YM-NATIVE-POINTWISE-BANACH-1

Statement:
Fix an index set I of sector/polymer pairs i=(Sigma,X), Banach spaces H_i (for example H^infty on fixed inner analytic domains), nonnegative distances d_i, and kappa_*>0. Define

B_pt={F=(F_i): sup_i exp(kappa_* d_i)||F_i||_{H_i}<infinity}

with norm
||F||_pt=sup_i exp(kappa_* d_i)||F_i||_{H_i}.

Then B_pt is Banach.

If a source theorem gives
||F_i||_{H_i}<=A exp(-kappa d_i)
for every i and kappa>=kappa_*, then
||F||_pt<=A.

Status:
PROVED.

Proof:
The isometric map F_i -> exp(kappa_*d_i)F_i identifies B_pt with the l^infinity product of the H_i, which is complete. The source-bound implication is immediate.

Purpose:
Provide a source-faithful matching Banach topology that uses exactly the form of Balaban-type pointwise analytic polymer estimates, postponing polymer entropy/summability to YM-NATIVE-TAIL-1.

---

## LEMMA ID: YM-ANALYTIC-COLLAR-1

Statement:
Let F be holomorphic on a complex Banach-space domain D_+ and suppose every point of a smaller domain D is surrounded, in each normalized complex direction h under consideration, by a complex disk of radius delta contained in D_+. Then

||D^m F(x)[h_1,...,h_m]||
 <= m! delta^{-m} ||F||_{H^infty(D_+)}

for x in D and normalized directions, with the standard multivariable Cauchy bound.

Status:
PROVED.

Purpose:
Explain how source H^infty activity bounds on a slightly larger Balaban analytic domain can supply derivative bounds on a smaller common domain without imposing a global all-field C^p supremum.

Yang-Mills gap:
Need a uniform analytic collar delta>0 over the matching interval and sector family; this has not been extracted.

---

## LEMMA ID: YM-NATIVE-MATCH-PRESERVATION-1

Statement:
Suppose a Balaban RT/R theorem supplies a fixed inductive density predicate P_k(D) whose local E/R/B/large-field components obey pointwise analytic bounds with common decay reserve kappa>kappa_*, and suppose:
(i) RT/R maps P_k into P_{k+1};
(ii) the analytic domains contain a common inner family U^*;
(iii) the source metrics/index families admit the same pointwise norm definition at consecutive scales after rescaling.

Then the component family of every D satisfying P_k lies in a bounded ball of the pointwise native Banach space of YM-NATIVE-POINTWISE-BANACH-1, and the image lies in the corresponding next-scale bounded ball. If the amplitude bounds are unchanged by the inductive predicate, this is a self-ball preservation statement after rescaling.

Status:
PROVED CONDITIONALLY ON (i)-(iii).

Known source support:
The extracted theorem shape of CMP119 Theorem 1 / CMP122-II Theorem 1 provides (i) at the level of their native inductive density class under stated small-coupling and constant restrictions.

Unverified:
(ii)-(iii) and the full numerical parameter dictionary. Therefore this is not yet an instantiated STEP_0 theorem.


---

## LEMMA ID: YM-COMPAT-CLOSED-1

Statement:
Let B be Banach and let {C_a:B->Y_a}_{a in A} be any family of bounded linear maps into normed spaces. Then

C = intersection_{a in A} ker C_a

is a closed linear subspace of B and therefore Banach with the inherited norm.

Status:
PROVED.

Proof:
Each ker C_a is closed because C_a is continuous. An arbitrary intersection of closed sets is closed. Closed linear subspaces of Banach spaces are Banach.

Use:
Verified support/gauge/covariance/normalization/restriction equalities may be imposed this way on the native coefficient product. This does NOT prove that Balaban's complete admissibility and overlap rules are all of this form.

---

## LEMMA ID: YM-RECON-GRAPH-1

Statement:
Let X,Y be metric spaces and Rec:X->Y continuous. Then

Graph(Rec)={(x,y): y=Rec(x)}

is closed in X x Y.

If Y is a Banach function space with closed positive cone Y_+, and N:Y->R is continuous, then

{(x,rho): rho=Rec(x), rho in Y_+, N(rho)=1}

is closed.

Status:
PROVED.

Purpose:
The actual Balaban matching state should be represented as a nonlinear reconstruction graph, not as the entire linear coefficient product.

Yang-Mills instantiation:
BLOCKED because continuity of the history sum defining Rec has not been established uniformly in cutoff/volume.

---

## LEMMA ID: YM-HISTORY-SUP-FAIL-1

Statement:
There is no constant C independent of N such that for all nonnegative families
(a_1,...,a_N),

sum_{i=1}^N a_i <= C max_i a_i.

Status:
PROVED.

Proof:
Take a_i=1. Then the left side is N and the right side is C.

Consequence:
A weighted l-infinity norm over Balaban domain histories does not by itself control the reconstruction sum over histories. Either exact disjointness, cutoff-uniform finite overlap, or weighted history summability is required.

---

## LEMMA ID: YM-HISTORY-WEIGHT-1

Statement:
Let sector contributions Sec_Sigma(U) obey

|Sec_Sigma(U)| <= tau_Sigma a_Sigma

and suppose there are positive weights w_Sigma such that

a_Sigma <= R / w_Sigma

and

H_hist :=
sup_U sum_{Sigma in Adm}
 |chi_Sigma(U)| tau_Sigma / w_Sigma
< infinity.

Then

sup_U |sum_Sigma chi_Sigma(U) Sec_Sigma(U)|
<= H_hist R.

Status:
PROVED.

Proof:
Triangle inequality followed by the two assumed majorants.

Special cases:
- exact disjointness with |chi|<=1 gives H_hist<=sup tau when at most one history contributes;
- overlap multiplicity <=C gives H_hist<=C sup tau for w=1;
- genuine branching requires a nontrivial history weight.

Yang-Mills instantiation:
UNKNOWN. No source-derived disjointness theorem, cutoff-uniform overlap constant, or complete history weight has been extracted.

---

## LEMMA ID: YM-TREE-COUNT-1

Statement:
Let G be a graph of maximum degree Delta<infinity and let d be a nonnegative tree-size metric on connected finite vertex sets. Assume

|X| <= a_0 + a_1 d(X)

with constants independent of volume/scale.

Then for every anchor B and n>=0,

#{X contains B: n<=d(X)<n+1}
<= C_count exp(c_count n)

with explicit constants depending only on Delta,a_0,a_1.

Status:
PROVED.

Proof:
Fix once and for all an ordering of vertices and neighbours. Every connected m-vertex set X containing B has a canonical spanning tree (choose the first one under the induced ordering) and a canonical depth-first traversal beginning at B. The traversal has exactly 2(m-1) edge steps and its set of visited vertices is X. Hence the map X -> canonical traversal is injective. Since each step has at most Delta choices,

#{connected X containing B: |X|=m}
<= Delta^{2(m-1)}.

If d(X)<n+1, coercivity gives

m=|X| <= M_n := floor(a_0+a_1(n+1)).

Therefore

#{X contains B: n<=d(X)<n+1}
<= sum_{m=1}^{M_n} Delta^{2(m-1)}.

For Delta>1,

sum_{m=1}^{M_n} Delta^{2(m-1)}
= (Delta^{2M_n}-1)/(Delta^2-1),

so one may take, for example,

c_count=2 a_1 log Delta

and a finite C_count depending only on Delta,a_0,a_1. The cases Delta<=1 are trivial.

Balaban relevance:
The source-audit ledger identifies CMP109 d_j(X) as shortest tree-graph length divided by M for a connected finite union of localization cubes. Thus the remaining Balaban-specific geometric input is the uniform coercivity

|X| <= a_0+a_1 d_j(X)

in the exact pi_j-cube convention. The lattice-animal combinatorics itself is no longer a blocker.

---

## LEMMA ID: YM-DIAMETER-COUNT-FAIL-1

Statement:
A metric that controls only diameter does not in general imply a shell count
#{X contains B: d(X) in [n,n+1)} <= C exp(c n)
for connected subsets of Z^d when d>=2.

Status:
PROVED.

Reason:
A box of side O(n) contains O(n^d) sites. Fix a connected backbone spanning the box and independently include many adjacent optional sites while maintaining connectedness. This gives exp(c n^d) distinct connected sets of diameter O(n), which cannot be bounded by C exp(c' n).

Purpose:
The native metric must be verified to control tree/cardinality complexity, not merely geometric diameter.

---

## LEMMA ID: YM-HISTORY-POLYMER-SEPARATION-1

Statement:
A polymer shell bound for fixed history,

sup_{Sigma,B}
#{X: B in X, n<=d_Sigma(X)<n+1}
<= C exp(c n),

does not imply a corresponding bound on pairs (Sigma,X).

Status:
PROVED.

Proof:
For any fixed X one may replicate the same X under arbitrarily many distinct history labels without changing d_Sigma(X). Thus the pair count can be arbitrarily larger unless the history family has an independent multiplicity/weight bound.

Consequence:
Polymer entropy and domain-history entropy are separate certificates in YM-MATCH-EXTRACT-1.

---

## LEMMA ID: YM-NATIVE-STATE-GRAPH-1

Statement:
Let B_pt be the native coefficient Banach product, C_lin its verified closed linear compatibility subspace, I_c a coupling interval, Q a coupling-profile Banach space, and D a density Banach space. If

Rec:I_c x Q x C_lin -> D

is continuous, define

M_adm =
{(c,q,F,rho):
 c in I_c,
 q in Q,
 F in C_lin,
 rho=Rec(c,q,F),
 rho>=0,
 N(rho)=1}.

Then M_adm is a closed nonlinear subset of the product state space whenever I_c is closed, the positive cone of D is closed, and N is continuous.

Status:
PROVED CONDITIONALLY ON continuity of Rec and the chosen density topology.

Important:
M_adm is not generally linear or affine because Rec contains exponentiation/integration and positivity. Thus calling the space of actual Balaban densities a closed linear subspace of B_pt would be incorrect.

Yang-Mills instantiation:
BLOCKED by history summability/reconstruction continuity, exact admissibility data, and a fixed analytic chart.

---

## LEMMA ID: YM-RAW-COMMON-DOMAIN-WARNING-1

Statement:
Suppose a family of analytic domains has radii r_j with inf_j r_j=0. Then there is no common raw ball of positive radius delta_* contained in every domain.

Status:
PROVED.

Balaban relevance:
The extracted CMP119 domain parameters have the schematic dependence
alpha_{r,j}=g_j C_r (log g_j^{-2})^{q_r}.
For historical UV scales along an asymptotically-free trajectory, g_j may approach zero as the UV cutoff is removed. Hence a positive cutoff-uniform common raw analytic core cannot be inferred and may fail.

Repair target:
Use normalized source-dependent charts into a fixed reference domain and prove uniform chart/transition distortion instead of intersecting all raw domains.

---

## LEMMA ID: YM-LOCAL-COUPLING-PROFILE-1

Statement:
A recursion of the form

c_{j-1}(x)=c_j(x)+b_j phi_j(x)

cannot in general be represented pointwise by a single scalar c_j unless phi_j is spatially constant.

Status:
PROVED.

Proof:
If phi_j(x) is nonconstant, then even a constant c_j(x) produces a nonconstant c_{j-1}(x).

Iteration-5 physical interpretation correction:
Primary CMP119 Eq. (2.24) directly shows that phi_j is a localization cutoff attached to Lambda_j:
phi_j in C_0^infinity(Lambda_j), equal to 1 on the prescribed shrunken interior. The source does not sum or integrate over phi_j.

Therefore the nonconstant profile need NOT be an independent physical state coordinate. After fixing a deterministic cutoff-selection rule Lambda -> phi_Lambda, the profile is DERIVED from the bulk coupling sequence and the domain history. An independently variable q may still be useful as an off-physical ambient extension for differentiation, but such an extension has to be constructed/proved and is not source-native.

The old ell_W coordinate remains outside the native matching construction.


---

## LEMMA ID: YM-RAW-PARTITION-1

Statement:
For each elementary CMP119 large/small-field decomposition of the form used in Eqs. (3.2), (3.3), and (3.16), with a finite cube set C and complementary pointwise conditions P_< and P_>= partitioning field space at a threshold, the family

chi_S =
 product_{cube in C\S} 1_{P_<}
 product_{cube in S}    1_{P_>=}

satisfies

chi_S in {0,1},
sum_{S subset C} chi_S = 1

pointwise.

Status:
PROVED, and the source instantiation is DIRECT PRIMARY VERIFIED.

Proof:
For each cube exactly one of the complementary conditions < threshold and >= threshold holds. Thus exactly one subset S, namely the set of cubes on which the >= condition holds, has chi_S=1.

Balaban relevance:
CMP119 Eqs. (3.2), (3.3), (3.16) are exactly of this form.

Important limitation:
After resummation these raw labels are not the labels of Eq. (2.18). Fixed raw P,Q sets producing the same Omega are resummed into zeta, while S is summed inside T. Therefore this lemma does NOT imply sum over compressed histories of chi_k(Omega_k)=1.

---

## LEMMA ID: YM-COMPRESSED-CHI-NOT-HISTORY-1

Statement:
In CMP119 Eq. (2.18), chi_k(Omega_k) is not a characteristic factor indexed by the complete compressed history H_k=(Omega_1,Lambda_1;...;Omega_k,Lambda_k;S_1,...,S_k).

Status:
PROVED FROM PRIMARY SOURCE STRUCTURE.

Reason:
Eq. (2.17) defines chi_k using only the terminal domain Omega_k. Immediately after Eq. (2.18), the source states that the S_j sums are inside T_k and that A_k depends on the full Omega/Lambda/S sequences. Sec. 3 further resums distinct raw P,Q choices producing the same Omega_{k+1} into zeta(Omega_{k+1}^c).

Consequence:
The identity
sum_H chi_H(U)=1
must not be asserted with chi_H identified with chi_k(Omega_k). The final history sum requires T/zeta/energy control.

---

## LEMMA ID: YM-HISTORY-RECON-ABSTRACT-1

Statement:
Let {A_H}_{H in H} be unital Banach algebras of scalar functions and D a Banach space. Let linear maps K_H:A_H->D satisfy

||K_H G||_D <= tau_H ||G||_{A_H}.

Let w_H>=1 and define

B_amb = {A=(A_H): ||A||_w:=sup_H w_H||A_H||_{A_H}<infinity}.

Assume

H_0 := sum_H tau_H < infinity,
H_1 := sum_H tau_H/w_H < infinity.

For ||A||_w<=R define

Rec(A)=sum_H K_H(exp A_H).

Then:
(1) the series converges absolutely in D;
(2)
||Rec(A)||_D <= e^R H_0;
(3) for ||A||_w,||B||_w<=R,

||Rec(A)-Rec(B)||_D
 <= e^R H_1 ||A-B||_w.

Status:
PROVED.

Proof:
Because w_H>=1,
||A_H||<=R and ||exp A_H||<=e^R, so absolute convergence and (2) follow from H_0.

For scalar pointwise multiplication,
exp a-exp b
=(a-b) integral_0^1 exp((1-t)b+t a) dt.
Hence
||exp A_H-exp B_H||
<=e^R||A_H-B_H||
<=e^R||A-B||_w/w_H.
Apply K_H and sum to obtain (3).

Purpose:
This is the precise absolute/Lipschitz reconstruction theorem needed after source history weights have been identified.

Yang-Mills instantiation:
PARTIAL. CMP122-II directly supplies a normalized local T operator estimate (1.75), an explicit admissible-history entropy majorant before (1.79), large-field penalties, and the local consequence (1.89). The unresolved step is to package these into one fixed local/relative density norm D and weights w_H with cutoff/volume/depth independent H_0,H_1.

---

## LEMMA ID: YM-T-LOCAL-MAJORANT-1

Statement:
Under the hypotheses of CMP122-II's local post-R large-field analysis, the localized T'_k operation satisfies

|(T'_k(X,(U,J))1)^(-1) T'_k(X,(U,J))F|
<= exp(3 sup|sigma|) sup|F|,

and the same source proves

T'_k(X)1
<= exp[-2(1+beta_0)^(-1)p_0(g_k)]

for the domain X in the construction.

Therefore

|T'_k(X)F|
<= exp(3 sup|sigma|)
   exp[-2(1+beta_0)^(-1)p_0(g_k)]
   sup|F|.

Status:
DIRECT PRIMARY VERIFIED / algebraic combination of CMP122-II (1.75) and (1.89).

Limitation:
This is a local T majorant, not yet a global Rec_k operator norm in the new ambient Banach topology.

---

## LEMMA ID: YM-HISTORY-ENTROPY-PAYMENT-1

Statement:
In the CMP122-II estimate leading to Eq. (1.79), the summation over admissible sequences contributes positive entropy factors of the form

exp{O(1)(M R_j)^(-d)|Z_j|}.

The large-field characteristic functions created at the same scales give negative exponential factors. After decomposition into M R_j-scale components Z_j^(i), their combined form includes penalties of the source shape

exp[
 -(1/2) gamma_0 A_1^2 p_0(g_j)^2
        (d'_j(Z_j^(i))+1)
 -2p_0(g_j)
].

The source combines these in (1.79), then proves an induction for the residual component cost kappa_j(Z); creation of a new large field supplies a new exp[-p_0(g_j)] factor. This yields, in particular, the local suppression (1.89).

Status:
DIRECT PRIMARY VERIFIED AS SOURCE MECHANISM.

Meaning:
History entropy is not an unaccounted combinatorial multiplicity in Balaban's proof. It is explicitly paid by large-field energy/smallness factors.

Limitation:
No single scalar p>s is extracted; the actual mechanism is multiscale and uses volume/tree costs plus p_0(g_j). Converting it to fixed history weights H_0,H_1 remains open.


---

## LEMMA ID: YM-BALABAN-TREE-COERCIVITY-1

Statement:
Let X be a CMP109 localization domain in dimension d, i.e. a connected finite union of pi_j-cubes. Let |X| denote the number of pi_j-cubes. With Balaban's metric d_j(X), directly defined as the shortest tree-graph length divided by M (equivalently after rescaling pi_j-cubes to unit cubes), one has

|X| <= 2^d (d_j(X)+1).

In d=4,

|X| <= 16(d_j(X)+1).

Status:
PROVED from the direct primary definition.

Proof:
CMP109 directly states that a shortest tree graph may be chosen to be formed by edges of cubes in X. After the source rescaling, pi_j-cubes are unit cubes, so such a finite tree of normalized length L=d_j(X) has L unit edges and L+1 graph vertices.

Every cube of X is intersected by the tree. If an intersection occurs along a cube edge, the cube contains an endpoint vertex of that edge; hence every cube of X contains at least one tree vertex. A lattice vertex belongs to at most 2^d unit cubes. Therefore the number of distinct cubes intersected by the tree is at most 2^d(L+1).

Thus |X|<=2^d(d_j(X)+1).

Uniformity:
The constant depends only on dimension, not on j, volume, or cutoff, because the source definition rescales pi_j-cubes to unit cubes.

---

## LEMMA ID: YM-BALABAN-TREE-SHELL-COUNT-1

Statement:
For d=4 CMP109 localization domains containing a fixed pi_j-cube B,

#{X contains B: n<=d_j(X)<n+1}
<= C_tree exp(c_tree n)

with explicit cutoff/scale/volume independent constants, for example

c_tree = 16 log 64 = 96 log 2,

C_tree = 64^32/63.

Status:
PROVED (very nonoptimal explicit constants).

Proof:
CMP109 connectivity is by common 3-dimensional wall, so the adjacency graph of 4D cubes has maximum degree Delta=8.

By YM-BALABAN-TREE-COERCIVITY-1,
if d_j(X)<n+1 then

|X|<=16(n+2).

For connected m-cube sets through B, the canonical spanning-tree depth-first encoding gives at most

Delta^{2(m-1)}=64^{m-1}

possibilities.

Therefore

#{X contains B: n<=d_j(X)<n+1}
<= sum_{m=1}^{16(n+2)}64^{m-1}
< 64^{16(n+2)}/63
= (64^32/63) exp((16 log64)n).

Purpose:
The fixed-history polymer/tree counting side-task is now closed. It does NOT count domain histories; the latter is controlled by the separate CMP122-II admissible-sequence entropy mechanism.


---

## LEMMA ID: YM-GLOBAL-H0-FAIL-1

Statement:
A global absolute history norm
H_0^global=sum_H tau_H
can grow exponentially with volume even when the history decomposition is an exact partition and every fixed local normalized observable is volume-independent.

Status:
PROVED.

Proof:
Take N independent binary blocks u_b in {0,1}. Let histories h in {0,1}^N and
chi_h(u)=prod_b 1_{u_b=h_b}.
Then sum_h chi_h(u)=1 pointwise and ||chi_h||_infinity=1, so
sum_h ||chi_h||_infinity=2^N.
For a product reference measure and local source supported on fixed A,
Z_N(J_A)/Z_N(0)=2^{-|A|} sum_{u_A} exp[J_A O(u_A)],
independent of N.

Consequence:
GLOBAL-HISTORY H_0 is too strong and must not be a necessary target for YM-HISTORY-NORM-1.

---

## LEMMA ID: YM-LOCAL-ACTIVITY-BANACH-1

Statement:
Let P_k be connected localization domains at scale k and A_X Banach spaces. For mu>=0 define

||K||_{loc,mu;k}
=
sup_{Delta in pi_k}
sum_{X contains Delta}
e^{mu d_k(X)} ||K_X||_{A_X}.

Then the space of K with finite norm is Banach.

Status:
PROVED.

Proof:
A norm-Cauchy sequence is coordinatewise Cauchy because each coordinate appears in an anchored sum. Let K_X be the coordinatewise limit. For every anchor Delta, Fatou's lemma for nonnegative series gives the anchored norm bound. Applying the same argument to differences gives convergence in norm.

---

## LEMMA ID: YM-SOURCE-GEOMETRIC-SUM-1

Statement:
For the CMP109 localization domains and sufficiently large lambda, the primary-source argument in Eq. (0.26) gives a scale/volume-uniform constant C_geo(lambda) such that

sup_{j,Delta in pi_j}
sum_{X in D_j: X contains Delta}
e^{-lambda d_j(X)}
<= C_geo(lambda)<infinity.

Consequently, if
||K_X||<=A e^{-Lambda d_j(X)}
and mu>=0 is such that Lambda-mu lies in the source summability regime, then

||K||_{loc,mu;j}
<= A C_geo(Lambda-mu).

Status:
DIRECT PRIMARY SOURCE + PROVED CONSEQUENCE.

Purpose:
This is the source-native bridge from pointwise exponential polymer decay to a local/relative volume-independent Banach activity norm. It supersedes the crude DFS constants when proving Banach summability.

---

## LEMMA ID: YM-T-HISTORY-LOCAL-H0-1

Statement:
Under the small-coupling hypotheses of CMP122-II, let a_0=2/(1+beta_0). The local post-R large-field operation satisfies the source bounds

T'_k(X)1 <= exp[-a_0 p_0(g_k)]

and, for an arbitrary large-field region, the source states the improved form with an additional tree penalty

T'_k(X)1
<= exp[-a_0 p_0(g_k)] exp[-lambda_T d_k(X)].

Hence for any mu such that lambda_T-mu lies in the CMP109 summability regime,

H_{0,T}^{loc}(mu)
:=
sup_Delta
sum_{X contains Delta}
e^{mu d_k(X)} |T'_k(X)1|

<= exp[-a_0 p_0(g_k)] C_geo(lambda_T-mu).

If g_k in (0,gamma] and p_min(gamma)=inf_{0<g<=gamma}p_0(g), then

H_{0,T}^{loc}(mu)
<= exp[-a_0 p_min(gamma)] C_geo(lambda_T-mu),

uniformly in volume, cutoff, and RG depth.

Status:
DIRECT PRIMARY SOURCE + PROVED SUMMATION CONSEQUENCE.

Operator version:
CMP122-II (1.75) multiplies the scalar bound by exp(3 sup|sigma|) for the normalized action on F.


Notation note:
Here lambda_T denotes the positive tree-decay coefficient in the source's improved arbitrary-region form following (1.89); no numerical value is assigned.

---

## LEMMA ID: YM-R-HISTORY-LOCAL-H0-1

Statement:
For the regular second-group connected activities produced by the CMP122-II exponentiated R-operation, Eq. (1.100) gives

|R'^(k)(X,(U,J))|
<= exp[-p_0(g_k)] exp[-kappa d_k(X)].

Therefore, if kappa-mu is in the CMP109 source summability regime,

sup_Delta
sum_{X contains Delta}
e^{mu d_k(X)}
sup_{(U,J)}|R'^(k)(X,(U,J))|

<= exp[-p_0(g_k)] C_geo(kappa-mu).

Status:
DIRECT PRIMARY SOURCE + PROVED SUMMATION CONSEQUENCE.

Limitation:
The boundary/large-field-connected intermediate activities in (1.99) use a relative metric d_{k,Y}; their final source placement into the B-sector has standard d_j decay, but an independent anchored theorem for that intermediate relative metric is not asserted here.

---

## LEMMA ID: YM-LOCAL-ACTIVITY-VECTOR-1

Iteration-7 corrected typed statement:
Let P be a countable family of connected polymers. For each gamma let C_gamma be a Banach space. For a finite block region A with dependency collar A^+, define

Gamma_A={gamma: supp(gamma) intersects A^+}

and the common local target

D_A^(mu)
 =
 l^1({C_gamma}_{gamma in Gamma_A}; e^{mu d(gamma)}),

i.e. vectors u=(u_gamma) with norm

||u||_{D_A^(mu)}
 =
 sum_{gamma in Gamma_A}
 e^{mu d(gamma)} ||u_gamma||_{C_gamma}.

For every gamma in Gamma_A define the canonical coordinate injection

iota_{gamma,A}:C_gamma -> D_A^(mu),

whose only nonzero coordinate is gamma. Then

||iota_{gamma,A}||
 =
 e^{mu d(gamma)}.

Let Phi_gamma map an ambient radius-R ball U_R into C_gamma. Define the TYPED local activity vector

Act_A(F)
 =
 sum_{gamma in Gamma_A}
 iota_{gamma,A} Phi_gamma(F).

Assume

h_0^(mu)
 =
 sup_B
 sum_{gamma: B in supp(gamma)}
 e^{mu d(gamma)}
 sup_{F in U_R} ||Phi_gamma(F)||
 < infinity,

and

h_1^(mu)
 =
 sup_B
 sum_{gamma: B in supp(gamma)}
 e^{mu d(gamma)}
 Lip_R(Phi_gamma)
 < infinity.

Then:
1. Act_A(F) converges absolutely in D_A^(mu);
2.
   ||Act_A(F)||_{D_A^(mu)}
   <= |A^+| h_0^(mu);
3.
   ||Act_A(F)-Act_A(G)||_{D_A^(mu)}
   <= |A^+| h_1^(mu) ||F-G||.

Status:
PROVED. This supersedes the untyped Iteration-6 formulation.

Proof:
The norm of each injected term is exactly
e^{mu d(gamma)}||Phi_gamma(F)||.
Every gamma in Gamma_A contains at least one anchor block B in A^+. Therefore

sum_{gamma in Gamma_A} (...)
<=
sum_{B in A^+}
sum_{gamma contains B} (...).

Apply h_0 or h_1.

Cylinder-function interpretation:
If C_gamma is a Banach space of cylinder functions on the local field variables of gamma, iota_{gamma,A} is NOT extension of that function to a common physical configuration space. It is the canonical injection into the l^1 direct-sum activity target. Any later synthesis/evaluation map from D_A^(mu) to actual local functions is a separate bounded map and must be proved.


---

## LEMMA ID: YM-REMOTE-HISTORY-CANCEL-1

Statement:
Let a post-resummation effective log-density be represented by connected activities K_X. For finite A define the local projection Pi_A K={K_X:X intersects A^+}, where A^+ is the source-dependency collar.

If two global post-resummation history configurations induce identical connected activities for all X intersecting A^+, then

Pi_A K = Pi_A K'.

In particular, changes in disconnected remote connected-history components do not alter the local relative log-density projection.

Status:
PROVED.

Proof:
Immediate from the definition of Pi_A.

Source-faithful basis:
CMP119 (2.19) factorizes T over disjoint large-field components. CMP122-II (1.90)-(1.99) resums and exponentiates connected localization components.

Limitation:
This is exact at the connected effective-action/history representation level. It is not a theorem that fully integrated local marginals are exactly independent of arbitrary remote changes.

---

## LEMMA ID: YM-SOURCE-RATIO-CANCEL-ABSTRACT-1

Statement:
Suppose a convergent connected expansion of a partition function has

log Z_Lambda(J)=C_Lambda(J)+sum_{C connected} phi_J(C).

Let J_A be local and assume
phi_{J_A}(C)=phi_0(C)
for every connected cluster C not connected to A.

Then

W_Lambda(J_A)
=
log Z_Lambda(J_A)-log Z_Lambda(0)

=
Delta C_A+
sum_{C connected to A}
[phi_{J_A}(C)-phi_0(C)].

Thus all completely remote connected clusters cancel exactly.

If

q_A=
sup_B sum_{C contains B}
|phi_{J_A}(C)-phi_0(C)|<infinity,

then

|W_Lambda(J_A)-Delta C_A|
<= |A| q_A,

uniformly in total volume.

Status:
PROVED ABSTRACTLY.

Yang-Mills instantiation:
NOT YET PROVED for the full source-dependent Balaban density. The selected connected-activity topology is designed so this theorem can be applied once marked/source activity bounds are available.

---

## LEMMA ID: YM-TREE-NORMALIZATION-AUDIT-1

Statement:
In CMP109 p.257 the pi_j cubes have side M in the lattice T_xi coordinates, xi=L^{-j}. The shortest tree length is divided by M; equivalently the source rescales the space so pi_j cubes become unit cubes and d_j is measured in that scale. Therefore no additional factor M is missing in d_j.

A cube-edge segment has physical length M xi, while its rescaled length is one. Hence the earlier coercivity

|X|<=2^d(d_j(X)+1)

and in d=4

|X|<=16(d_j(X)+1)

has the correct normalization.

Status:
DIRECT PRIMARY VERIFIED / PROVED.

Recommendation:
For local Banach summability use CMP109 Eq. (0.26) directly rather than the crude explicit DFS shell constant.


---

## LEMMA ID: YM-HISTORY-JACOBIAN-1

Statement:
Let P_in,P_out be polymer families on a countable block lattice. Let A_eta,C_gamma be Banach spaces and define anchored norms

||h||_in
 =
 sup_a
 sum_{eta contains a}
 v_eta ||h_eta||_{A_eta},

||k||_out
 =
 sup_b
 sum_{gamma contains b}
 u_gamma ||k_gamma||_{C_gamma},

with positive weights v_eta,u_gamma.

Let M=(M_{gamma eta}) be bounded linear coordinate maps
M_{gamma eta}:A_eta->C_gamma.
For every pair with M_{gamma eta} nonzero choose a selector
s(gamma,eta) in supp(eta).

Define

S(M)
 =
 sup_b
 sum_a
 sup_{eta contains a}
 [
   v_eta^(-1)
   sum_{gamma contains b: s(gamma,eta)=a}
   u_gamma ||M_{gamma eta}||
 ].

If S(M)<infinity, then the coordinate series

(Mh)_gamma=sum_eta M_{gamma eta} h_eta

defines a bounded map B_in->B_out and

||M||_{B_in->B_out} <= S(M).

Status:
PROVED.

Proof:
For fixed output anchor b,

sum_{gamma contains b}u_gamma||(Mh)_gamma||
<=
sum_{gamma contains b}sum_eta u_gamma||M_{gamma eta}||||h_eta||.

Assign each pair (gamma,eta) to the unique a=s(gamma,eta). For fixed a this is

sum_{eta contains a}
 ||h_eta||
 sum_{gamma contains b:s(gamma,eta)=a}
 u_gamma||M_{gamma eta}||.

Bound the bracket divided by v_eta by its supremum over eta containing a and use

sum_{eta contains a}v_eta||h_eta||<=||h||_in.

Sum over a and take sup over b.

Interpretation:
This is the correct orientation of the weighted Schur test for an anchored l^1-in-polymer / l^infinity-in-anchor norm. A bare sup_gamma sum_eta condition is not the native operator norm.

---

## LEMMA ID: YM-HISTORY-JACOBIAN-QUASILOCAL-1

Statement:
In the setting of YM-HISTORY-JACOBIAN-1, suppose there are C,nu>0 such that for every blocks a,b,

sup_{eta contains a}
 [
 v_eta^(-1)
 sum_{gamma contains b:s(gamma,eta)=a}
 u_gamma ||M_{gamma eta}||
 ]
<=
C exp[-nu dist(a,b)].

If

C_lat(nu):=sup_b sum_a exp[-nu dist(a,b)]<infinity,

then

||M||_{B_in->B_out}
<= C C_lat(nu).

Status:
PROVED.

Purpose:
This is the exact quasi-local Jacobian target to extract from Balaban history resummation. It simultaneously controls multiplicity of input polymers, output polymers, and spatial propagation.

---

## LEMMA ID: YM-BANACH-CAUCHY-C1-1

Statement:
Let X,Y be complex Banach spaces. Suppose H is holomorphic on the open ball
B_X(0,R+delta), delta>0, and

sup_{||F||<R+delta} ||H(F)||_Y <= M.

Then for every ||F||<=R,

||D H_F||_{X->Y}
<= M/delta.

More generally, if the uniform output bound is M_Rplusdelta on that ball, use that value.

Status:
PROVED.

Proof:
For ||h||_X=1 and any complex continuous functional y* with ||y*||=1, apply the scalar Cauchy formula to
z -> y*(H(F+z h))
on |z|<delta. Taking the dual supremum gives the result.

Loss-of-decay use:
If source estimates only bound H in a weaker output space B_{mu_out}, this theorem gives
H:B_{mu_in}->B_{mu_out}
C^1 with exactly that decay loss. No coordinatewise Jacobian estimate is needed for bounded C^1, though the Jacobian theorem is still needed for marked quasi-locality.

---

## LEMMA ID: YM-EXP-LOG-BANACH-1

Statement:
Let A be a unital Banach algebra with submultiplicative norm.

Exponentiation:
For all K,H in A,

D exp(K)[H]
 =
 integral_0^1 exp((1-t)K) H exp(tK) dt,

and

||D exp(K)|| <= exp(||K||).

Logarithm:
For ||X||<1,

log(1+X)=sum_{n>=1}(-1)^(n+1)X^n/n

converges absolutely, is analytic, and

||D log(1+X)|| <= 1/(1-||X||).

Status:
PROVED.

Proof:
Standard norm-convergent power-series / Duhamel differentiation. For log, differentiating the series termwise gives n terms at order n, whose total norm is bounded by ||H|| ||X||^(n-1).

Limitation:
Applying this directly to polymer activities requires a separately proved Banach-algebra/convolution estimate for the chosen anchored polymer product. Balaban's cluster expansion provides a different, source-native route to the log map.

---

## LEMMA ID: YM-CLUSTER-CAUCHY-C1-1

Statement:
Let B_in^kappa and B_out^{kappa-epsilon} be complex connected-activity Banach spaces. Suppose a cluster/resummation map C is represented in finite volume by analytic polynomial/power-series expressions in the input activities, and the series converges absolutely and uniformly in volume on the larger ball

||F||_{B_in^kappa}<R+delta

with

sup ||C(F)||_{B_out^{kappa-epsilon}} <= M

independent of volume/cutoff/scale.

Then the infinite-volume/uniform map is holomorphic on that ball and, on ||F||<=R,

||D C_F||
<= M/delta.

Status:
PROVED UNDER THE STATED UNIFORM-CONVERGENCE HYPOTHESIS.

Proof:
Uniform limits on smaller balls of Banach-valued holomorphic maps are holomorphic. Apply YM-BANACH-CAUCHY-C1-1.

Purpose:
This is the precise theorem needed to turn a STRICT cluster-expansion smallness reserve into a uniform C^1 estimate.

---

## LEMMA ID: YM-T-NORMALIZED-DERIV-1

Statement:
Let

T_A(F)=integral dnu(U) W(U) exp(A(U)) F(U)

on a fixed local history component, with Z_A=T_A(1)>0 and normalized operator

Tbar_A(F)=T_A(F)/Z_A.

Assume on a complex neighborhood

|Tbar_A(G)| <= C_T ||G||_infinity

for all bounded local G.

Then for any bounded action perturbation h,

D Tbar_A[h](F)
 =
 Tbar_A(hF)-Tbar_A(h)Tbar_A(F),

and

|D Tbar_A[h](F)|
<=
(C_T+C_T^2)||h||_infinity||F||_infinity.

Also

D Z_A[h]=T_A(h)

and

|D Z_A[h]|
<=
Z_A C_T ||h||_infinity.

Status:
PROVED.

Balaban instantiation:
CMP122-II (1.75) provides C_T=exp(3 sup|sigma|) for the normalized local T'_k operation, subject to its source hypotheses. Combined with the source scalar/tree suppression of Z_A=T'_k(X)1, a local action derivative inherits the same tree decay up to the local restriction norm of h.

Limitation:
This proves C^1 of a FIXED local T component with respect to analytic action perturbations. It does not prove C^1 of the complete history branching/resummation map, where characteristic domains and chart changes also vary.

---

## LEMMA ID: YM-LOCAL-RESTRICTION-1

Statement:
Let h=(h_eta) be in an anchored input activity space with mu_in>=0. For a finite block set X^+ define the local summed perturbation

r_X h
 =
 sum_{eta: supp(eta) intersects X^+} h_eta

viewed in any common local sup-norm space in which restriction/extension of each h_eta has operator norm at most 1.

Then

||r_X h||_infinity
<=
|X^+| ||h||_in.

Status:
PROVED.

Proof:
Overcount by anchors:
sum_{eta intersects X^+}||h_eta||
<=
sum_{a in X^+}
sum_{eta contains a}||h_eta||
<=
|X^+| ||h||_in.

Use:
When |X^+| has at most polynomial/linear growth in d(X), this factor can be absorbed by a positive loss of exponential tree decay.

---

## LEMMA ID: YM-T-SECTOR-C1-1

Statement:
Assume the local T'_k component satisfies:
(i) CMP122-II normalized operator bound with C_T=exp(3 sup|sigma|);
(ii) scalar/tree suppression
Z_X:=|T'_k(X)1|
<= A_T exp[-lambda_T d_k(X)];
(iii) the action input enters T'_k(X) only through a local collar X^+ and the restriction map obeys YM-LOCAL-RESTRICTION-1;
(iv) |X^+|<=C_col(1+d_k(X)).

Then the derivative with respect to the local action-activity input satisfies

||D T'_k(X)||
<=
A_T C_T C_col(1+d_k(X))
exp[-lambda_T d_k(X)]

as a map from the anchored input norm to the local output operator norm.

Consequently, for any mu_out and epsilon>0 such that
lambda_T-mu_out-epsilon is in the CMP109 geometric summability regime,

sup_Delta
sum_{X contains Delta}
e^{mu_out d_k(X)}
||D T'_k(X)||
<=
A_T C_T C_col C_epsilon
C_geo(lambda_T-mu_out-epsilon),

where

C_epsilon:=sup_{t>=0}(1+t)e^{-epsilon t}<infinity.

Status:
PROVED CONDITIONAL ON (ii)-(iv) in the precise chosen local charts.

Source status:
(i) and the existence of source tree suppression are primary-verified.
The exact collar constant C_col and one fixed normalized chart needed for (iii)-(iv) have NOT been source-instantiated.

Therefore:
fixed-component T derivative mechanism = PROVED;
full uniform T-sector C^1 in one fixed Balaban Banach chart = NOT YET PROVED.

---

## LEMMA ID: YM-DECAY-LOSS-1

Statement:
Let source activities/derivatives have pointwise tree decay exp[-kappa d(X)] and suppose CMP109 geometric summability is finite for every lambda>lambda_geo,* (or for a stated source threshold regime). If the output anchored exponent is mu_out=kappa-epsilon, then the anchored sum is finite whenever

epsilon>lambda_geo,*.

More generally any additional polynomial factor P(d(X)) is absorbed by reserving a further epsilon_1>0 because

sup_{t>=0} P(t)e^{-epsilon_1 t}<infinity.

Status:
PROVED.

Interpretation:
A positive loss of decay is natural and should be budgeted explicitly. No numerical minimal epsilon is claimed because the source threshold lambda_geo,* has not been numerically extracted.

---

## LEMMA ID: YM-SOURCE-MARK-TANGENT-1

Corrected statement:
Let J_A be a Banach source space and B_in an anchored activity Banach space. Let

jmath_A:J_A -> B_in

be a linear insertion map.

If jmath_A is bounded and Hscr:U subset B_in->B_out is C^1, then for every source vector J in J_A,

D Hscr_F[jmath_A(J)]

is a well-defined activity tangent and

||D Hscr_F[jmath_A(J)]||
<=
||D Hscr_F|| ||jmath_A|| ||J||_{J_A}.

If, additionally, the history Jacobian satisfies a quasi-local Schur bound and jmath_A has a local/quasi-local support bound, the marked output inherits the corresponding locality estimate.

Status:
PROVED ABSTRACTLY.

Yang-Mills status:
The source space and insertion map needed for a physical gauge-invariant observable have not been source-instantiated here. In particular, the boundedness of jmath_A is UNKNOWN. A physical source J_A must not be identified with an activity tangent without this interface.

---

## LEMMA ID: YM-CLUSTER-JACOBIAN-SCHUR-1

Statement:
Let Hscr:B_in(mu_in)->B_out(mu_out) be a map with components Hscr_gamma taking values in C_gamma. Fix R,delta,A,kappa>0.

Assume for every ||F||_in<R+delta:

(A1) component bound:
||Hscr_gamma(F)||_{C_gamma}
<= A exp[-kappa d(gamma)];

(A2) holomorphy:
Hscr_gamma is holomorphic as a function on the B_in(mu_in) ball;

(A3) collar locality:
there is a scale-independent integer r_col>=0 such that the coordinate derivative
M_{gamma eta}(F)=D_eta Hscr_gamma(F)
vanishes unless
supp(eta) subset N_{r_col}(supp(gamma))
(or any source-specific condition implying dist(supp eta,supp gamma)<=r_col and allowing a selector within r_col of gamma).

Then for ||F||_in<=R, coordinate Cauchy gives

||M_{gamma eta}(F)||
<=
(A/delta)
e^{mu_in d(eta)}
e^{-kappa d(gamma)}.                                    (J1)

Suppose further that the source polymer geometry has the two-anchor consequence:
for any blocks a,b and gamma meeting b and lying within r_col of a,

d(gamma)
>= dist(a,b)-c_col

with c_col depending only on r_col.

Choose nu>0 such that

lambda_res
 :=
 kappa-mu_out-nu

lies in the CMP109 geometric summability regime.

Then the weighted Jacobian kernel satisfies

sup_{eta contains a}
[
 e^{-mu_in d(eta)}
 sum_{gamma contains b:s(gamma,eta)=a}
 e^{mu_out d(gamma)}
 ||M_{gamma eta}(F)||
]
<=
(A/delta)
e^{nu c_col}
C_geo(lambda_res)
e^{-nu dist(a,b)}.                                      (J2)

Consequently, by YM-HISTORY-JACOBIAN-QUASILOCAL-1,

||D Hscr_F||_{B_in(mu_in)->B_out(mu_out)}
<=
(A/delta)
e^{nu c_col}
C_geo(lambda_res)
C_lat(nu).                                               (J3)

Status:
PROVED UNDER (A1)-(A3) AND THE STATED SOURCE-GEOMETRIC INPUT.

Proof:
For a local unit vector h in A_eta, its coordinate injection I_eta h has

||I_eta h||_in
<=
e^{mu_in d(eta)}.

Thus the complex line
F+z I_eta h
remains in the larger input ball for

|z|<delta e^{-mu_in d(eta)}.

Scalar/Banach Cauchy applied to Hscr_gamma gives (J1).

Multiply by the Schur weights. The factor e^{mu_in d(eta)} cancels exactly against the input weight. Split

e^{-(kappa-mu_out)d(gamma)}
=
e^{-nu d(gamma)}
e^{-lambda_res d(gamma)}.

The collar geometry gives
e^{-nu d(gamma)}
<=e^{nu c_col}e^{-nu dist(a,b)}.
Sum the remaining factor with C_geo(lambda_res), obtaining (J2). Apply the quasi-local Schur lemma.

Importance:
This theorem converts exactly the three source ingredients one expects from a constructive cluster expansion—
(1) analytic reserve,
(2) output tree decay,
(3) connected support/collar locality—
into a uniform quasi-local C^1 operator bound.

---

## LEMMA ID: YM-GRADED-ACTIVITY-BANACH-1

Statement:
Let the scale/history index i range over an arbitrary countable set and let A_i be arbitrary Banach spaces (they need not be the same space or be defined on the same analytic field domain). Let each i carry a polymer support supp(i), a tree size d(i), and scale-local anchors.

The anchored product

B_grad(mu)
 =
 {F=(F_i), F_i in A_i:
   sup_anchor
   sum_{i incident to anchor}
     e^{mu d(i)}||F_i||_{A_i}<infinity}

is Banach.

Status:
PROVED.

Proof:
Same coordinatewise completeness plus Fatou argument as for YM-LOCAL-ACTIVITY-BANACH-1.

Interpretation:
A common raw field domain is NOT logically necessary merely to define a single fixed Banach space of ACTIVITY COEFFICIENTS. One may use a graded product whose coordinate Banach spaces are the native H-infinity spaces at their historical scales.

However:
- RG chart transitions must still be bounded maps between the coordinate spaces;
- field/source differentiation inside A_i still needs a uniform analytic collar;
- a first source mark J_A is a field/source derivative, so the normalized-chart problem remains essential for YM-UV-SOURCE-MATCH-1.

Thus the fixed-reference-domain blocker is essential for source-variable C^1, but not for the bare coefficient-space definition.

---

## LEMMA ID: YM-RPRIME-CLUSTER-C1-1

Statement:
Let C_R be the connected cluster/log submap taking pre-exponentiation activities F(X') to post-exponentiation connected activities R'(X).

Assume there exists a source-uniform activity convergence radius r_*>0 such that:

(i) for every rho<r_* and every input family in the anchored input ball ||F||<=rho, the finite-volume cluster expansion defining C_R converges absolutely with bounds uniform in volume/cutoff/scale;

(ii) on that ball the output components satisfy

||C_R(F)_X||
<=
C_R0 rho
exp[-kappa_out d(X)]

with C_R0 independent of volume/cutoff/scale;

(iii) derivative support obeys the exact-union/collar condition of YM-CLUSTER-JACOBIAN-SCHUR-1.

Then for every r<rho<r_* the map

C_R:
B_in(mu_in; ||F||<r)
 ->
B_out(mu_out)

is uniformly C^1 whenever
kappa_out-mu_out-nu
lies in the CMP109 summability regime for some nu>0.

A valid explicit derivative constant is obtained from YM-CLUSTER-JACOBIAN-SCHUR-1 with

A=C_R0 rho,
delta=rho-r.

Status:
PROVED UNDER (i)-(iii).

CMP122-II instantiation:
The source proves a sufficiently-small pre-exponentiation activity regime, convergent exponentiation, and the decay ledger

kappa_in=(1+beta)kappa,
kappa_out=(1+beta/2)kappa.

Thus the source-visible decay loss is

epsilon_R=(beta/2)kappa.

Iteration-9 update:
The regular R' source instantiation is now resolved in YM-RPRIME-C1-1 by:
- the exact cube-or-wall compatibility from CMP116 (2.11);
- the explicit native KP radius r_*=(a/9)exp[-16a-sqrt(7)theta];
- the source-crosswalk identifying legacy (7.13)[I] with published CMP116 (2.13), hence exact-union native coordinates.

This abstract lemma remains useful as a generic template, but its former regular-R' source-instantiation gap is superseded.


---

## LEMMA ID: YM-RPRIME-NATIVE-COMPAT-1

Statement:
For the Balaban polymer system used in the exponentiation of CMP122-II (1.90), define the cube carrier

Car(eta)
=
{B in pi_k : B is a pi_k cube belonging to eta},

and its closed wall-neighborhood

Supp_*(eta)
=
N_wall[Car(eta)].

The native incompatibility relation is

eta not~ eta'
iff
Car(eta') cap Supp_*(eta) != empty,

equivalently iff eta cap eta' contains either a pi_k cube or a full (d-1)-wall of a pi_k cube.

In d=4,

#Supp_*(eta)<=9|eta|.

Hence one may take

C_inc=9.

Moreover every incompatibility edge admits a tree stitching cost at most

c_stitch=sqrt(7)

in the rescaled unit-cube geometry.

Status:
PROVED BY DIRECT PRIMARY SOURCE + ELEMENTARY GEOMETRY.

Source:
Published CMP116 Eq. (2.11) defines the incompatibility function zeta(Z,Z') to vanish exactly when Z cap Z' contains a cube or a wall of a cube. CMP122-II (1.90) is exponentiated by the same legacy cluster construction; the source-crosswalk is recorded in YM-RPRIME-NATIVE-COORD-1.

Proof of C_inc:
A four-dimensional cube has 8 wall-neighbors. Therefore the closed one-step wall-neighborhood of a carrier of |eta| cubes contains at most 9|eta| cubes.

Proof of c_stitch:
If eta not~ eta', choose carrier cubes B,B' that either coincide or share a wall. A shortest tree inside each polymer meets the chosen cube. In the worst case B union B' is a rectangular box with side lengths 2,1,1,1 and diameter sqrt(7). Join the two meeting points inside this box.

---

## LEMMA ID: YM-RPRIME-NATIVE-TREE-STITCH-1

Statement:
Let eta_1,...,eta_n be localization polymers whose native Balaban incompatibility graph is connected and set

gamma=union_i eta_i.

Then in d=4,

d_k(gamma)
<=
sum_i d_k(eta_i)
+
sqrt(7)(n-1).

Status:
PROVED.

Proof:
Choose a spanning tree of the incompatibility graph. Apply the per-edge stitching bound c_stitch=sqrt(7) from YM-RPRIME-NATIVE-COMPAT-1 to its n-1 edges.

---

## LEMMA ID: YM-RPRIME-NATIVE-TREE-KP-BALL-1

Statement:
Let the input activity family F be indexed by the native Balaban localization polymers with cube-or-wall incompatibility. Define

||F||_{mu_in}
=
sup_B
sum_{eta: B in Car(eta)}
e^{mu_in d_k(eta)}||F_eta||_A.

Fix a,theta>0 satisfying

16a+theta<=mu_in.

Define

g_theta(eta)
=
theta d_k(eta)+sqrt(7)theta,

q_native(rho)
=
exp(16a+sqrt(7)theta)rho,

and

r_*^native
=
(a/9)exp[-16a-sqrt(7)theta].

Then every ball

||F||_{mu_in}<r_*^native

satisfies the generalized Kotecky-Preiss criterion with

a_KP(eta)=a|eta|,
g=g_theta.

The exact-union connected logarithm converges absolutely and normally in the anchored output norm

||K||_theta
=
sup_B sum_{X containing B}
e^{theta d_k(X)}||K_X||_A,

uniformly in finite volume and scale. In particular it is Banach-holomorphic.

Status:
PROVED.

Proof:
The d=4 coercivity gives

|eta|<=16(d_k(eta)+1).

Thus

a|eta|+g_theta(eta)
<=
mu_in d_k(eta)+16a+sqrt(7)theta,

so the rooted activity sum is at most q_native(rho).

By YM-RPRIME-NATIVE-COMPAT-1,

sum_{eta not~ eta_0}
||F_eta|| e^{a|eta|+g_theta(eta)}
<=
9|eta_0|q_native(rho).

If rho<r_*^native, then

9q_native(rho)<a,

which is the KP hypothesis.

For a connected cluster, YM-RPRIME-NATIVE-TREE-STITCH-1 gives

e^{theta d_k(union_i eta_i)}
<=
product_i e^{theta d_k(eta_i)+sqrt(7)theta}.

The rooted generalized KP bound therefore controls the complete anchored output norm and provides a normal-convergence majorant. Banach-valued Weierstrass yields holomorphy.

Remark:
The earlier radius
a exp[-16a-2theta]
belongs only to the stricter carrier-overlap-only toy/canonical compatibility. It is superseded for native Balaban compatibility.

---

## LEMMA ID: YM-RPRIME-SOURCE-INPUT-1

Statement:
Fix the CMP122-II exponentiation step and frozen domains Y_1,...,Y_m. Let

Ycal=union_i Y_i.

For a regular pre-exponentiation activity eta=X' disjoint from Ycal, CMP122-II (1.97) gives

||F_eta^src||
<=
e^{-p_0(g_k)}
e^{-(1+beta)kappa d_k(eta)}.

Choose lambda_in with C_geo(lambda_in)<infinity and set

mu_in=(1+beta)kappa-lambda_in>0.

Then

||F^src||_{mu_in}
<=
C_geo(lambda_in)e^{-p_0(g_k)}.

Because primary CMP119 has

p_0(g)=A_0(log g^{-2})^{p_*},

p_*>0,

the right-hand side tends to zero as the fixed small-coupling threshold gamma tends to zero.

Consequently, for the corrected native radius r_*^native of YM-RPRIME-NATIVE-TREE-KP-BALL-1 there exists a fixed gamma_R'>0 such that

r
:=
C_geo(lambda_in)e^{-p_0(gamma_R')}
<
r_*^native,

and for all g_k<=gamma_R',

||F^src||_{mu_in}<=r.

Status:
DIRECT PRIMARY SOURCE + PROVED SUMMATION CONSEQUENCE.

---

## LEMMA ID: YM-RPRIME-NATIVE-COORD-1

Statement:
The local coefficients R'^(k)_Bal(X) appearing after CMP122-II (1.98) are the exact-union connected Ursell/log coefficients of the polymer activities F(X') from (1.90):

R'^(k)_Bal(X)
=
sum_{n>=1} 1/n!
sum_{X'_1,...,X'_n:
     union_i X'_i=X}
rho^T(X'_1,...,X'_n)
product_i F(X'_i),

where rho^T is the connected-graph coefficient built from the cube-or-wall compatibility function zeta.

Equivalently,

R'^(k)_Bal(X)=H_X^can(F)

coordinatewise.

Status:
PROVED BY SOURCE CROSSWALK.

Source crosswalk:
1. CMP122-II says its first exponentiation operation is the Mayer expansion "the same as in (7.1) [I]".
2. The bibliography identifies [I] with CMP109 (1987).
3. The published CMP109 version ends before the localization/cluster-expansion construction and explicitly says that construction is deferred to the next paper.
4. That next paper, published CMP116 (1988), Sect. 2 begins with the Mayer expansion Eq. (2.1).
5. CMP116 Eq. (2.11) gives the polymer compatibility function, Eq. (2.12) the connected logarithm, and Eq. (2.13) the localized coefficient with the exact-union condition union_i Z_i=X.
6. CMP122-II then states that R'^(k)(X) is given by the convergent series "(7.13) [I]" with proper notational changes.

The two matching anchors (legacy 7.1 <-> published 2.1 and legacy 7.13 <-> published 2.13), together with CMP109's explicit deferral of the cluster construction, identify the legacy manuscript numbering with the separately published CMP116 Sect. 2.

No extra localization map L occurs after the connected Ursell sum in this route.

Corollary:
For the native derivative kernel,

D_eta R'^(k)_Bal(X)=0

unless

eta subset X.

Thus the derivative-support collar is

r_col=0.

---

## LEMMA ID: YM-RPRIME-C1-1

Statement:
Fix source parameters beta,kappa and choose lambda_in such that

C_geo(lambda_in)<infinity,

mu_in=(1+beta)kappa-lambda_in>0.

Choose a,theta>0 with

16a+theta<=mu_in,

and define the corrected native radius

r_*=
(a/9)exp[-16a-sqrt(7)theta].

Choose gamma_R'>0 sufficiently small that

r=C_geo(lambda_in)e^{-p_0(gamma_R')}<r_*,

and set

rho=(r+r_*)/2.

Then for every scale k with 0<g_k<=gamma_R', the native regular/second-group Balaban coefficient map

R'^(k)_Bal:
B_r(B_in,k(mu_in))
->
B_out,k(theta)

is the restriction of a Banach-holomorphic map on the r_*-ball and hence is C^1. With

q_native(rho)
=
e^{16a+sqrt(7)theta}rho,

one has

sup_{||F||_in<=r}
||D R'^(k)_Bal(F)||
<=
C_R'^C1
:=
q_native(rho)/(rho-r).

The bound is independent of volume, UV cutoff and RG depth.

Status:
PROVED FOR THE NATIVE REGULAR R' CLUSTER/LOG COEFFICIENT MAP.

Dependencies:
YM-RPRIME-NATIVE-COMPAT-1,
YM-RPRIME-NATIVE-TREE-STITCH-1,
YM-RPRIME-NATIVE-TREE-KP-BALL-1,
YM-RPRIME-SOURCE-INPUT-1,
YM-RPRIME-NATIVE-COORD-1,
YM-BANACH-CAUCHY-C1-1.

Stronger quasi-local Jacobian:
If there exist mu_out>=0 and nu>0 such that

lambda_res
=
theta-mu_out-nu

satisfies

C_geo(lambda_res)<infinity,

then the exact-union support theorem and component Cauchy estimate imply

sup_{eta contains a}
[
e^{-mu_in d_k(eta)}
sum_{X contains b:s(X,eta)=a}
e^{mu_out d_k(X)}
||D_eta R'^(k)_Bal,X(F)||
]

<=
[q_native(rho)/(rho-r)]
C_geo(lambda_res)
e^{-nu dist_k(a,b)}.

Hence

||D R'^(k)_Bal(F)||_{B_in(mu_in)->B_out(mu_out)}
<=
[q_native(rho)/(rho-r)]
C_geo(lambda_res)
C_lat(nu).

Status of stronger statement:
PROVED UNDER THE EXPLICIT ADDITIONAL DECAY-BUDGET CONDITION
C_geo(theta-mu_out-nu)<infinity.

No numerical Balaban kappa threshold is claimed.

Non-claims:
No upstream Eq. (1.91) derivative theorem, no boundary B' theorem, no full history map, no physical source insertion, no K_0, and no STEP_0.


---

## LEMMA ID: YM-RPRIME-191-TRANSCRIPTION-1

Statement:
On a fixed CMP122-II history/domain sector, the pre-exponentiation activity of Eq. (1.91) is

F(X')
=
sum_{q>=0}
sum'_{ {X_{j_1},...,X_{j_q}} }
sum'_D
[
 product_{Y in D} int_0^1 dt(Y)
]
[
 product_{h=1}^q T'_k(X_{j_h})
]
[
 product_{Y in D}V(Y)
]
exp[
 sum_{Y in D}t(Y)V(Y)
].

The primed sums are exactly over those frozen localization-domain data whose special connected component, defined immediately before (1.90), is X'.

Status:
DIRECT PRIMARY SOURCE VERIFIED.

Source:
CMP122-II printed pp.387-388, Eqs. (1.90)-(1.91).

Consequences:
- Eq. (1.91) sees only the already-formed local T'_k operators and V(Y) activities.
- raw E/R/B coordinates do not occur explicitly;
- all constituents of a term contributing to F(X') lie in the component X'.

---

## LEMMA ID: YM-RPRIME-191-FROZEN-CHART-1

Statement:
Fix all hard determining-set/history/characteristic data Sigma used in the CMP122-II construction around (1.90)-(1.91). Vary only the analytic coefficient functions/operators T'_X,V_Y while keeping their localization labels fixed.

Then the index fiber

I_Sigma(X')
=
{(q,Xvec,D): these frozen localization domains determine exactly X'}

is independent of the coefficient values.

Thus Eq. (1.91) defines an analytic coefficient map on an open Banach fiber over Sigma without differentiating hard characteristic boundaries.

Status:
PROVED FROM THE SOURCE DEFINITION.

Reason:
CMP122-II says Omega_k and the determining domains are defined by the field/determining set, while after (1.91) the prime is a condition on the localization domains themselves. It is not a condition on the numerical values of T'_X or V_Y.

Important:
No Frechet derivative across two different hard history sectors is claimed. The full state is a disjoint union of Banach fibers, not one linear history space.

---

## LEMMA ID: YM-RPRIME-191-ELEMENTARY-DERIV-1

Statement:
For one frozen raw index I=(q,Xvec,D), define

W_D(V,t)=sum_{Y in D}t(Y)V_Y,

P_D(V)=product_{Y in D}V_Y,

G_D(V;t)=P_D(V)exp W_D(V,t),

T_Xvec=product_{h=1}^q T'_{X_{j_h}},

Phi_I(T',V)
=
int_{[0,1]^D}
T_Xvec[G_D(V;t)]dt.

Then:

1. if eta not in D,
   D_eta G_D=0;

2. if eta in D,

D_eta G_D[h]
=
exp(W_D)
[
 h product_{Y in D\{eta}}V_Y
 +
 t(eta)h product_{Y in D}V_Y
];

3. for a T'-coordinate eta,

D_eta T_Xvec[H]
=
sum_{r:X_{j_r}=eta}
T'_{X_{j_1}}...T'_{X_{j_{r-1}}}
H
T'_{X_{j_{r+1}}}...T'_{X_{j_q}};

4.

D Phi_I
=
int
[
 (D T_Xvec)G_D
 +
 T_Xvec(D G_D)
].

Status:
PROVED.

No derivative of the frozen index set occurs.

---

## LEMMA ID: YM-T-LOCAL-COEFF-C1-1

Statement:
Let a local T-operation be written

T_X(K)F=int e^K F dmu_X,

tau_X(K)=T_X(K)1,

bar T_X(K)=tau_X(K)^{-1}T_X(K).

On an open local coefficient domain where tau_X is nonzero,

D tau_X(K)[h]=T_X(K)h,

D bar T_X(K)[h]F
=
bar T_X(K)(hF)
-
bar T_X(K)(h)bar T_X(K)(F).

If

||bar T_X||_op<=C_T,

then

||D bar T_X(K)[h]||_op
<=
(C_T+C_T^2)||h||.

Also

D T_X(K)[h]F=T_X(K)(hF).

Status:
PROVED.

CMP122-II instantiation:
(1.74) gives nonvanishing in the source analytic regime and (1.75) gives

C_T<=exp(3 sup|sigma|).

The later proof bounds |sigma| by 1, so e^3 is a valid crude local source constant.

This proves the local exponential/integration submap is not the current upstream blocker.

---

## LEMMA ID: YM-RPRIME-191-C1-1

Statement:
Fix one frozen source sector Sigma.

Let B_red,k^(Sigma)=T_k^(Sigma) direct-sum V_k^(Sigma), with the mixed source-majorant + anchored norms defined in rprime_upstream_c1.md. The weighted-sup part preserves the exact pointwise source majorants and the anchored part gives volume-independent local summability.

Assume the fixed source threshold is chosen with strict slack in every smallness inequality used in the absolute-value proof of CMP122-II (1.92)-(1.97). Then there exists

R_src>1

independent of volume, cutoff and k such that the same proof converges uniformly for every coefficient family in

B_{R_src}(B_red,k^(Sigma)).

Choose the threshold still smaller so that the resulting inflated regular output satisfies

||F||_{B_F(mu_F)}
<=rho_F<r_*^native

on that whole ball.

Let Pi_reg project to localization components X' in the regular lane. Then the exact regular Eq. (1.91) map

M_1.91^reg
=
Pi_reg M_1.91:
B_{R_src}(B_red,k^(Sigma))
->
B_F(mu_F)

is Banach-holomorphic and, on the inner unit ball,

sup ||D M_1.91||
<=
C_1.91
:=
rho_F/(R_src-1).

Status:
PROVED UNDER THE EXPLICIT "STRICT SOURCE-MAJORANT SLACK" HYPOTHESIS.

Why the hypothesis is source-available:
the source permits alpha to be made arbitrarily small by shrinking the coupling threshold and p_0(gamma) -> infinity; all inequalities used in (1.92)-(1.97) are strict sufficient smallness inequalities. Therefore the threshold can be chosen inside, rather than on the boundary of, the allowed majorant region.

No physical U/J analytic collar is used.

---

## LEMMA ID: YM-RPRIME-191-SUPPORT-1

Statement:
For the reduced Eq. (1.91) coefficient map,

D_eta F(X')=0

unless

eta subset X',

for both T'-coordinates and V-coordinates.

Status:
PROVED.

Proof:
The source sentence after (1.91) says a term is summed into F(X') exactly when its localization-domain constituents determine the connected localization domain X'. Hence every constituent domain occurring in that term belongs to X'. A coordinate derivative can be nonzero only when that coordinate occurs in the term.

Corollary:
The reduced coefficient-map collar is

r_col=0.

---

## LEMMA ID: YM-RPRIME-UPSTREAM-PRODUCER-1

Desired statement:
On each frozen history sector Sigma, construct a product anchored Banach space

B_up
=
Tpot
direct-sum K_E
direct-sum K_R
direct-sum K_B
direct-sum K_old
direct-sum ...

and prove a uniform bounded C^1 producer

P_Sigma:
B_up
->
B_red,

P_Sigma(raw)=(T'(raw),V(raw)).

Current status:
OPEN / BLOCKER.

What is already proved:
- fixed restriction/pullback is linear in a coefficient function;
- the local exponential/integration map producing T' from an already-localized exponent insertion is C^1 (YM-T-LOCAL-COEFF-C1-1);
- the exact (T',V)->F map is C^1 (YM-RPRIME-191-C1-1).

First missing operator:
the coefficient-space map

P_V:
(K_E,K_R,K_B,K_old,...)
->
(V(Y))_Y,

where CMP122-II pp.376-377 perform localization, E/R renormalization, resummation, and addition/subtraction before obtaining the local bounds (1.68),(1.69).

The source proves output analyticity in physical fields and pointwise bounds, but this notebook has not yet extracted a bounded difference/Frechet estimate for P_V in an anchored coefficient norm.

---

## LEMMA ID: YM-RPRIME-UPSTREAM-C1-1

Desired raw theorem:
(T,E,R,B,local history coefficients)->F is uniformly bounded C^1.

Status:
PARTIAL / BLOCKED AT YM-RPRIME-UPSTREAM-PRODUCER-1.

Proved reduced theorem:
(T',V)->F_reg is uniformly bounded C^1 on the frozen-history reduced coefficient chart. No boundary B' output is included.

Downstream composition:
Since audited F->R'_Bal has

sup||D H_R'||<=C_R',

one has for the reduced coordinates

||D(H_R' circ M_1.91)||
<=
C_R' C_1.91.

For the full raw map the analogous bound requires

C_P
=
sup||D P_Sigma||<infinity,

which is not yet proved.
